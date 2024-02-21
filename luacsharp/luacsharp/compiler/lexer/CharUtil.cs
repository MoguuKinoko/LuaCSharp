namespace luacsharp.compiler.lexer
{
    class CharUtil {

        static bool isWhiteSpace(char c) {
            switch (c) {
                case '\t':
                case '\n':
                // case 0x0B: // \v
                case '\f':
                case '\r':
                case ' ':
                    return true;
            }
            return false;
        }

        static bool isNewLine(char c) {
            return c == '\r' || c == '\n';
        }

        static bool isDigit(char c) {
            return c >= '0' && c <= '9';
        }

        static bool isLetter(char c) {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';
        }

    }
}