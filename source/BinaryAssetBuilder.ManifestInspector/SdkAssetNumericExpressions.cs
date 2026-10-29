using System.Globalization;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: evaluate source numeric arithmetic without executing code or changing legacy diagnostic expression profiles.
internal sealed class SdkAssetNumericExpressions
{
    private readonly string text;
    private readonly IReadOnlyDictionary<string,string> definitions;
    private int at,depth;
    private SdkAssetNumericExpressions(string expression,IReadOnlyDictionary<string,string> definitions){text=expression;this.definitions=definitions;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse finite scalar arithmetic with visible case-sensitive definitions and ordinary operator precedence. */
    //-------------------------------------------------------------------------------------------------
    internal static string Evaluate(string expression,IReadOnlyDictionary<string,string> definitions)
    {
        if(expression.Length>512||!expression.StartsWith('='))throw new InvalidDataException("Numeric expression exceeds its syntax bound.");
        var parser=new SdkAssetNumericExpressions(expression[1..],definitions);double value=parser.Sum();parser.White();
        if(parser.at!=parser.text.Length||!double.IsFinite(value))throw new InvalidDataException("Invalid or non-finite numeric expression: "+expression);
        return value.ToString("G17",CultureInfo.InvariantCulture);
    }
    // Reborn: addition and subtraction bind after products and quotients.
    private double Sum(){double value=Product();while(true){White();if(Take('+'))value+=Product();else if(Take('-'))value-=Product();else return value;}}
    // Reborn: division by zero is diagnosed at the original source slot.
    private double Product(){double value=Atom();while(true){White();if(Take('*'))value*=Atom();else if(Take('/')){double divisor=Atom();if(divisor==0)throw new InvalidDataException("Division by zero in numeric expression.");value/=divisor;}else return value;}}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept numeric literals, visible numeric definitions, unary signs and bounded parentheses; reject calls and unit guessing. */
    //-------------------------------------------------------------------------------------------------
    private double Atom()
    {
        if(++depth>64)throw new InvalidDataException("Numeric expression nesting exceeds 64.");
        try{White();if(Take('+'))return Atom();if(Take('-'))return -Atom();if(Take('(')){double value=Sum();White();if(!Take(')'))throw new InvalidDataException("Missing closing parenthesis in numeric expression.");return value;}
            int start=at;if(Take('$')){start=at;while(at<text.Length&&(char.IsAsciiLetterOrDigit(text[at])||text[at]=='_'))at++;string name=text[start..at];if(!definitions.TryGetValue(name,out string? literal)||!double.TryParse(literal,NumberStyles.Float,CultureInfo.InvariantCulture,out double scalar)||!double.IsFinite(scalar))throw new InvalidDataException("Numeric definition is missing or not a finite scalar: $"+name);return scalar;}
            while(at<text.Length&&(char.IsAsciiDigit(text[at])||text[at]=='.'))at++;
            if(at<text.Length&&text[at] is 'e' or 'E'){at++;if(at<text.Length&&text[at] is '+' or '-')at++;while(at<text.Length&&char.IsAsciiDigit(text[at]))at++;}
            if(at==start||!double.TryParse(text[start..at],NumberStyles.Float,CultureInfo.InvariantCulture,out double number)||!double.IsFinite(number))throw new InvalidDataException("Unsupported numeric expression token near: "+text[start..]);return number;
        }finally{depth--;}
    }
    // Reborn: whitespace has no semantic effect between arithmetic tokens.
    private void White(){while(at<text.Length&&char.IsWhiteSpace(text[at]))at++;}
    private bool Take(char token){if(at>=text.Length||text[at]!=token)return false;at++;return true;}
}
