using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Magicube.ElasticSearch.Test {
    [ElasticsearchType(IdProperty = "packageId")]
    public class LayoutPackage : ISearchModel<string> {
        [Ignore, JsonIgnore]
        public string   Id         { get; set; }
	
        [Keyword(Name = "packageId")]
	    public string   PackageId  { get; set; }
	
	    [Text(Name="packageName")]
	    public string   PackageName{ get; set; }
	
	    [Text(Name="type")]
	    public string   Type       { get; set; }
	
	    [Text(Name="keyword")]
	    public string[] Keyword    { get; set; }
	
	    [Number(NumberType.Integer, Name="total")]
	    public int      Total      { get; set; }
	
	    [Text(Name="uploadDate")]
	    public DateTime UploadDate { get; set; }
	
	    [Boolean(Name="enabled")]
	    public bool     Enabled    { get; set; }
	
	    [Number(NumberType.Integer, Name="vip")]
	    public int      Vip        { get; set; }
	
	    [Number(NumberType.Integer, Name="top")]
	    public int      Top        { get; set; }
    }

    public class FooSearchModel : ISearchModel<int> {
        public int          Id          { get; set; }
                                        
        [Text(Name = "Text")]           
        public string       Text        { get; set; }
                                        
        [Text(Name = "Title")]          
        public string       Title       { get; set; }
                                        
        [Number(Name= "UserId")]        
        public int          UserId      { get; set; }
                                        
        [Text(Name = "Keywords")]       
        public List<string> Keywords    { get; set; }
                                        
        [Text(Name = "Thumbnail")]      
        public string       Thumbnail   { get; set; }
        
        [PropertyName("Text_vector")]
        public float[]      Text_vector { get; set; }

        [Number(NumberType.Long, Name = "Sort")]
        public long         Sort        { get; set; }

        [Boolean(Name = "IsTemplate")]
        public bool         IsTemplate  { get; set; }

        [Date(Name = "CreateAt ")]
        public DateTime CreateAt { get; set; }
    }
}
