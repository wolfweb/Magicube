﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Medias.Hls {
    public class M3UFileInfo {
        public DateTime?            ProgramDateTime { get; set; }
        public int?                 TargetDuration  { get; set; }
        public int?                 MediaSequence   { get; set; }
        public string               PlaylistType    { get; set; } = default!;
        public IList<M3UMediaInfo>  MediaFiles      { get; set; } = default!;
        public bool?                AllowCache      { get; set; }
        public IList<M3UStreamInfo> Streams         { get; set; } = default!;
        public int?                 Version         { get; set; }
        public M3UKeyInfo           Key             { get; set; } = default!;
        public M3UMediaInfo         Map             { get; set; }
        //当原始的m3u8中的数据 不满足需求的时候 可以通过自定义的数据 进行操作
        public object               UserData        { get; set; }

        public static M3UFileInfo CreateVodM3UFileInfo() {
            M3UFileInfo m3UFileInfo = new() {
                PlaylistType = "VOD"
            };
            return m3UFileInfo;
        }

        public M3UFileInfo(M3UFileInfo m3UFileInfo) : this() {
            Key            = m3UFileInfo.Key;
            Version        = m3UFileInfo.Version;
            Streams        = m3UFileInfo.Streams.ToList();
            MediaFiles     = m3UFileInfo.MediaFiles.ToList();
            PlaylistType   = m3UFileInfo.PlaylistType;
            MediaSequence  = m3UFileInfo.MediaSequence;
            TargetDuration = m3UFileInfo.TargetDuration;
        }

        public M3UFileInfo() {
        }
    }
}