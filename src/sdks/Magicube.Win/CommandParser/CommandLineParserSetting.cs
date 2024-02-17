using System;

namespace Magicube.Win {
    public class CommandLineParserSetting {
        public StartChar Start { get; set; }
        public SplitChar Split { get; set; }

        public CommandLineParserSetting() {
            Start = StartChar.Any;
            Split = SplitChar.Any;
        }

        public CommandLineParserSetting(StartChar start, SplitChar split) {
            Start = start;
            Split = split;
        }

        public Char GetStartChar() {
            switch (Start) {
                case StartChar.Slash:
                case StartChar.Any:
                    return '/';
                case StartChar.Dash:
                    return '-';
            }
            throw new Exception("Undefined StartChar: " + Start);
        }

        public Char GetSplitChar() {
            switch (Split) {
                case SplitChar.Colon:
                case SplitChar.Any:
                    return ':';
                case SplitChar.Equals:
                    return '=';
            }
            throw new Exception("Undefined SplitChar: " + Split);
        }

        public Boolean ValidStart(Char ch) {
            switch (Start) {
                case StartChar.Slash:
                    return ch == '/';
                case StartChar.Dash:
                    return ch == '-';
                case StartChar.Any:
                    return ch == '/' || ch == '-';
            }
            return false;
        }

        public Boolean ValidSplit(Char ch) {
            switch (Split) {
                case SplitChar.Equals:
                    return ch == '=';
                case SplitChar.Colon:
                    return ch == ':';
                case SplitChar.Any:
                    return ch == '=' || ch == ':';
            }
            return false;
        }
    }

}
