using System.Text.RegularExpressions;

namespace luacsharp.compiler.lexer
{
    public class Lexer
    {
        public string _chunk; //源代码
        public string _chunkName; //源文件名
        public int _line; //当前行号
        
        private static string reOpeningLongBracket = "^\\[=*\\[";

        public Lexer(string chunk, string chunkName)
        {
            _chunk = chunk;
            _chunkName = chunkName;
        }
        
        public Lexer newLexer(string chunk, string chunkName)
        {
            return new Lexer(chunk, chunkName);
        }

        public void NextToken(int line, int kind, string token)
        {
            skipWhiteSpaces();
            if (_chunk.Length == 0)
            {
                return line, TOKEN_EOF,"EOF";
            }
            
            switch (_chunk[0])
            {
                case ';':
                    next(1);
                    return _line, TOKEN_SEP_SEMI, "";
                case ',':
                    next(1);
                    return _line, TOKEN_SEP_COMMA, "";
            }
        }
        
        private void SkipWhiteSpaces()
        {
            while (_chunk.Length > 0)
            {
                if (_chunk.StartsWith("--"))
                {
                    SkipComment();
                }
                else if (_chunk.StartsWith("\r\n") || _chunk.StartsWith("\n\r"))
                {
                    _chunk = _chunk.Substring(2);
                    _line += 1;
                }
                else if (CharUtil.IsNewLine(_chunk[0]))
                {
                    _chunk = _chunk.Substring(1);
                    _line += 1;
                }
                else if (CharUtil.IsWhiteSpace(_chunk[0]))
                {
                    _chunk = _chunk.Substring(1);
                }
                else
                {
                    break;
                }
            }
        }
        
        private void SkipComment()
        {
            _chunk = _chunk.Substring(2); // 跳过 --

            // 长注释？
            if (_chunk.StartsWith("["))
            {
                if (Regex.Match(_chunk, reOpeningLongBracket) != null)
                {
                    ScanLongString();
                    return;
                }
            }

            // 短注释
            while (_chunk.Length > 0 && !CharUtil.IsNewLine(_chunk[0]))
            {
                _chunk = _chunk.Substring(1);
            }
        }
        
    }
}