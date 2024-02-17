using Magicube.Core;
using Magicube.TestBase;
using Newtonsoft.Json.Linq;
using System.Linq;
using Xunit;

namespace Magicube.Core.Test {
    public class JsonExtensionTest {
        public JsonExtensionTest() { 

        }
        [Fact]
        public void Func_Json_Search_Modify_Test() {
            string json = @"{
              'channel': {
                'title': 'Star Wars',
                'link': 'http://www.domain1.com',
                'description': 'Star Wars blog.',
                'obsolete': 'Obsolete value',
                'item': [
                    'http://www.domain2.com'
                ]
              }
            }";

            var obj = JObject.Parse(json);
            var resources = obj.GetsByValue(x => x.StartsWith("http"));
            Assert.True(resources.Count == 2);
            Assert.True(resources.First().Fields.Count == 1);

            resources.First().Carrier[resources.First().Fields[0]] = "http://wolfweb.com";
            Assert.True(obj.GetValue("channel").ToObject<JObject>().GetValue("link").ToString() == "http://wolfweb.com");

            resources[1].Carrier[resources[1].ArrayField][0] = "http://wolfweb.com";
            Assert.True(obj.GetValue("channel").ToObject<JObject>().GetValue("item").ToObject<JArray>()[0].ToString() == "http://wolfweb.com");

            resources = obj.GetsByKey(x => x == "channel");
            Assert.True(resources.Count == 1);
            Assert.True(resources.First().Carrier.ContainsKey("title"));
        }

        [Fact]
        public void Func_Json_Search_Test() {
            string json = @"{
                'schema':[
                {
                   'model':{
                      'name': 'name',
                      'value': 'wolfweb'
                   }
                },{
                   'model':{
                      'name': 'title',
                      'value': 'haha'
                   }
                }
            ]}";
            var obj = JObject.Parse(json);
            var resources = obj.GetsByKey(x => x == "model");
            Assert.True(resources.Count == 2);

            var arr = obj.Value<JArray>("schema");
            resources = arr.GetsByKey(x => x == "model");
            Assert.True(resources.Count == 2);
        }

        [Fact]
        public void Func_Json_Serialize_To_ParentObject() {
            var str = Json.Stringify(new Foo());
            var foo1 = str.JsonToObject<IFoo>();
            var foo2 = str.JsonToObject<AbstractFoo>();
            var foo3 = str.JsonToObject<Foo>();
            Assert.NotNull(foo1);
            Assert.NotNull(foo2);
            Assert.NotNull(foo3);
            Assert.Equal(foo1.Name, foo2.Name);
        }
    }
}
