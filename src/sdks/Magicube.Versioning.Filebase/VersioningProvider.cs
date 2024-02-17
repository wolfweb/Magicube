using LibGit2Sharp;
using Magicube.Core;
using Magicube.Versioning.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Magicube.Versioning.Filebase {
    public class VersioningProvider : IVersioningProvider {
        private const string BranchName = "origin";

        private readonly ILogger _logger;
        private readonly Repository _repository;
        private readonly VersioningOption _option;
        private readonly IVersioningMessageService _versionMessageService;

        public VersioningProvider(
            ILogger<IVersioningProvider> logger,
            IOptions<VersioningOption> options,
            IVersioningMessageService versionMessageService
        ) {
            _logger = logger;
            _option = options.Value;

            if (_option.Folder.IsNullOrEmpty()) throw new VersioningException("未初始化文件版本控制目录");

            _versionMessageService = versionMessageService;

            if (!Repository.IsValid(options.Value.Folder)) {
                Repository.Init(options.Value.Folder);
            }

            _repository = new Repository(options.Value.Folder);
            if (!options.Value.RemoteUrl.IsNullOrEmpty()) {
                _repository.Network.Remotes.Add(BranchName, options.Value.RemoteUrl);
                _repository.Branches.Update(_repository.Head,
                        b => b.Remote = BranchName,
                        b => b.UpstreamBranch = "refs/heads/master");
            }
        }

        public void AddOrUpdate(IVersioningContent content) {
            var file = Path.Combine(_option.Folder, $"{content.Key}");
            var signature = new Signature(_option.UserName, _option.UserEmail, content.UpdateAt.GetValueOrDefault(content.CreateAt));
            bool isCreate = false;
            if (!File.Exists(file)) {
                isCreate = true;
            }

            var datas = Encoding.UTF8.GetBytes(content.Content);
            using (var stream = File.Open(file, FileMode.OpenOrCreate)) {
                stream.Write(datas);
            }
            Commands.Stage(_repository, file);
            _repository.Commit(_versionMessageService.GetMessage(isCreate), signature, signature);
        }

        public VersioningContentChanges Compare(VersioningHistory v1, VersioningHistory v2) {
            var blob1   = _repository.Lookup<Blob>(v1.VersioningContentId);
            var blob2   = _repository.Lookup<Blob>(v2.VersioningContentId);
            var changes = _repository.Diff.Compare(blob1, blob2);
            var result  = new VersioningContentChanges(changes.LinesAdded, changes.LinesDeleted);

            result.AddedLines.AddRange(changes.AddedLines.Select(x => new ChangeLine(x.LineNumber, x.Content)));
            result.DeletedLines.AddRange(changes.DeletedLines.Select(x => new ChangeLine(x.LineNumber, x.Content)));

            return result;
        }

        public string GetVersionContent(VersioningHistory history) {
            return _repository.Lookup<Blob>(history.VersioningContentId).GetContentText();
        }

        public IEnumerable<VersioningHistory> Query(IVersioningContent content) {
            return _repository.Commits.QueryBy(content.Key).Select(x => new VersioningHistory {
                At       = x.Commit.Committer.When.DateTime,
                Key      = content.Key,
                Desc     = x.Commit.MessageShort,
                VersioningContentId   = x.Commit.Tree[content.Key].Target.Sha,
                VersioningId = x.Commit.Id.ToString()
            });
        }

        public void RecoveryTo(VersioningHistory history) {
            var commit = _repository.Lookup<Commit>(history.VersioningId);
            _repository.Reset(ResetMode.Hard, commit);
        }

        public void Remove(IVersioningContent content) {
            var signature = new Signature(_option.UserName, _option.UserEmail, content.UpdateAt.GetValueOrDefault(DateTime.Now));
            var file = Path.Combine(_option.Folder, $"{content.Key}");
            Commands.Remove(_repository, file, true);

            _repository.Commit($"delete {content.Key}", signature, signature);
        }

        public void SyncToRemote() {
            var remote = _repository.Network.Remotes.FirstOrDefault(x => x.Name == BranchName);
            if (remote == null) {
                throw new VersioningException("未设置远程服务器");
            }

            _repository.Network.Push(_repository.Head);
        }

        public void SyncFromRemote() {
            var signature = new Signature(_option.UserName, _option.UserEmail, DateTime.Now);
            var remote = _repository.Network.Remotes.FirstOrDefault(x => x.Name == BranchName);
            if (remote == null) {
                throw new VersioningException("未设置远程服务器");
            }

            MergeResult mergeResult = Commands.Pull(_repository, signature, new PullOptions());
            _logger.LogDebug($"同步远程数据时异常=>{mergeResult.Status}, {mergeResult.Commit.Message}");
        }
    }
}