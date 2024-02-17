using Magicube.Cache.Abstractions;
using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Eventbus;
using Magicube.LightApp.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Magicube.LightApp.Wechat {
    public class WechatUserService : ILightAppUserService {
        public const string Identity = "weixin";

        private readonly ICacheProvider _cacheProvider;
        private readonly IEventProvider _eventProvider;
        private readonly SenparcWeixinSetting _senparcWeixinSetting;
        private readonly IRepository<WechatUser, long> _wechatUserRep;

        public WechatUserService(
            IOptions<SenparcWeixinSetting> options, 
            IRepository<WechatUser, long> wechatUserRep,
            IEventProvider eventProvider, 
            IServiceProvider serviceProvider) {
            _eventProvider        = eventProvider;
            _wechatUserRep        = wechatUserRep;
            _senparcWeixinSetting = options.Value;
            _cacheProvider        = serviceProvider.GetService<ICacheProvider>(DefaultCacheProvider.Identity);
        }

        public string Key => Identity;

        public async Task<LightAppUserViewModel> GetOrAddUser(string code) {
            var jsonResult = SnsApi.JsCode2Json(_senparcWeixinSetting.WxOpenAppId, _senparcWeixinSetting.WxOpenAppSecret, code);
            if (jsonResult.errcode == ReturnCode.请求成功) {
                Expression<Func<WechatUser, bool>> query = user => jsonResult.unionid.IsNullOrEmpty() ? 
                                user.OpenId == jsonResult.openid : 
                                user.UnionId == jsonResult.unionid;

                var wechatUser = _wechatUserRep.All.SingleOrDefault(query);
                if(wechatUser == null) {
                    wechatUser = new WechatUser {
                        OpenId   = jsonResult.openid,
                        UnionId  = jsonResult.unionid,
                        NickName = "匿名用户",
                        UserType = LightAppUserType.Probation
                    };

                    await _eventProvider.OnCreatingAsync(new EventContext<LightAppUserEntity>(wechatUser));
                    await _wechatUserRep.InsertAsync(wechatUser);
                }

                _cacheProvider.GetOrAdd(BuildKey(wechatUser.UserId), () => jsonResult.session_key, TimeSpan.FromMinutes(5));

                return new LightAppUserViewModel {
                    Id               = wechatUser.UserId,
                    Avator           = wechatUser.AvatarUrl,
                    UserName         = wechatUser.NickName,
                    LightAppUserType = wechatUser.UserType
                };
            }
            throw new LightAppException(jsonResult.errmsg) { 
                AccessTokenOrAppId = _senparcWeixinSetting.WxOpenAppId,
            };
        }

        public async Task Profile(long userId, LigthAppUserDataViewModel viewModel) {
            if (_cacheProvider.TryGet(BuildKey(userId), out string v)) {
                var result = DecodeUserData(v, viewModel);
                var wechatUser = result.JsonToObject<WechatUser>();
                await UpdateWechatUserProfile(userId, wechatUser);
            }
            else if(!viewModel.Code.IsNullOrEmpty()){
                var jsonResult = SnsApi.JsCode2Json(_senparcWeixinSetting.WxOpenAppId, _senparcWeixinSetting.WxOpenAppSecret, viewModel.Code);

                if (jsonResult.errcode == ReturnCode.请求成功) {
                    var result = DecodeUserData(jsonResult.session_key, viewModel);
                    var wechatUser = result.JsonToObject<WechatUser>();
                    await UpdateWechatUserProfile(userId, wechatUser);
                }
                else {
                    throw new LightAppException(jsonResult.errmsg) {
                        AccessTokenOrAppId = _senparcWeixinSetting.WxOpenAppId,
                    };
                }
            }

            throw new LightAppException("请求的数据不完整，请确保code、iv、encryptedData全部包含数据") {
                AccessTokenOrAppId = _senparcWeixinSetting.WxOpenAppId,
            };
        }

        private async Task UpdateWechatUserProfile(long userId, WechatUser user) {
            var wechatUser = await _wechatUserRep.GetAsync(userId);

            wechatUser.NickName  = user.NickName;
            wechatUser.AvatarUrl = user.AvatarUrl;
            wechatUser.City      = user.City;
            wechatUser.Province  = user.Province;
            wechatUser.Gender    = user.Gender;
            wechatUser.Language  = user.Language;
            wechatUser.Country   = user.Country;

            await _eventProvider.OnUpdatingAsync(new EventContext<LightAppUserEntity>(wechatUser));

            await _wechatUserRep.UpdateAsync(wechatUser);
        }

        private string DecodeUserData(string sessionKey, LigthAppUserDataViewModel viewModel) {
            return EncryptHelper.DecodeEncryptedData(sessionKey, viewModel.EncryptedData, viewModel.Iv); 
        }

        private string BuildKey(long id) => $"LightAppUser:{Identity}:{id}";
    }
}
