using System.Globalization;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 예제 1: 타입은 저장할 수 있는 값과 가능한 연산을 정합니다.
int stock = 12;
long population = 8_000_000_000L;
double average = 82.5;
decimal price = 12.50m;
bool available = stock > 0;
char grade = 'A';
string product = "노트";
Console.WriteLine($"상품: {product}, 재고: {stock}, 판매 가능: {available}");
Console.WriteLine($"인구: {population}, 평균: {average:F1}, 등급: {grade}");

// 예제 2: var도 컴파일할 때 고정된 타입을 가집니다.
var quantity = 3;
decimal total = price * quantity;
const decimal TaxRate = 0.10m;
Console.WriteLine($"금액: {total:F2}, 세금: {total * TaxRate:F2}");

// 예제 3: nullable 값 타입은 값 없음도 표현합니다.
int? optionalScore = null;
Console.WriteLine($"미입력 점수: {optionalScore ?? -1}");
optionalScore = 90;
Console.WriteLine($"입력 점수: {optionalScore.Value}");

// 예제 4: 지역 변수에는 읽기 전에 값을 대입해야 합니다.
int count;
count = 5;
count = count + 2;
Console.WriteLine($"변경된 수량: {count}");
