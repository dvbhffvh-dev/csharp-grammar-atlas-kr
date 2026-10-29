# 03. 연산자와 안전한 형 변환

[이전: 02 변수와 자료형](../02-VariablesTypes/README.md) · [다음: 04 문자열](../04-Strings/README.md) · [전체 목차](../../README.md)

## 시작 전에

int, double, decimal, bool과 변수 대입을 알고 있어야 합니다.
C# 14 / .NET 10을 사용하며 명령은 저장소 최상위에서 실행합니다.
Visual Studio에서는 Chapter03을 시작 프로젝트로 선택하고 Ctrl+F5를 누르세요.

## 학습 목표

- 피연산자 타입에 따라 달라지는 계산 결과를 예측합니다.
- 논리 연산의 단락 평가로 안전한 조건을 만듭니다.
- TryParse와 checked로 실패와 범위 초과를 처리합니다.

## 실행 방법

```powershell
dotnet run --project chapters/03-OperatorsConversions
dotnet run --project chapters/03-OperatorsConversions -- --answers
```

## 1. 연산자는 값을 조합하는 규칙입니다

`a + b`에서 +는 연산자이고 a와 b는 피연산자입니다.
같은 +도 정수에서는 덧셈, 문자열에서는 연결이라는 의미가 있습니다.
연산을 읽을 때는 기호뿐 아니라 양쪽의 타입도 확인해야 합니다.

| 분류 | 연산자 | 의미 |
| --- | --- | --- |
| 산술 | `+ - * / %` | 사칙연산과 나머지 |
| 대입 | `= += -= *= /=` | 저장과 복합 대입 |
| 비교 | `== != < <= > >=` | bool을 만드는 비교 |
| 논리 | `&&`, `\|\|`, `!` | 조건 결합과 부정 |
| 증감 | `++ --` | 1 증가 또는 감소 |

`=`는 대입이고 `==`는 같은지 비교합니다.
`count += 2`는 단순한 int 변수에서 `count = count + 2`와 같은 목적입니다.
`2 + 3 * 4`는 곱셈이 먼저여서 14이며, `(2 + 3) * 4`는 20입니다.
중요한 계산에는 괄호를 적어 의도를 분명하게 표현하세요.

후위 `x++`는 증가 전 값을 식의 결과로 사용합니다.
전위 `++x`는 증가 후 값을 사용합니다.
둘을 복잡한 하나의 식에 섞기보다 증가 문장을 분리하면 추적하기 쉽습니다.

## 2. 실습 1 — 정수 나눗셈과 나머지

[Program.cs](Program.cs)의 첫 예제는 7을 2로 나눕니다.
두 피연산자가 int이면 결과도 int이며 소수 부분을 0 방향으로 잘라냅니다.
따라서 7 / 2는 3이고 -7 / 2는 -3입니다.

```csharp
int total = 7;
int people = 2;
double wrong = total / people;
double correct = (double)total / people;
```

1. wrong의 오른쪽은 먼저 int끼리 계산되어 3이 됩니다.
2. 나중에 double에 저장해도 버린 0.5는 돌아오지 않습니다.
3. correct는 계산 전에 total을 double로 바꾸므로 3.5입니다.

나머지 `7 % 2`는 1입니다.
분을 시간과 남은 분으로 나누거나 짝수 여부를 검사할 때 사용합니다.
정수에서 0으로 나누면 예외가 발생하므로 분모를 검증해야 합니다.

## 3. 암시적 변환과 명시적 변환

`int count = 10; long total = count;`처럼 허용된 변환은 자동으로 일어납니다.
하지만 암시적 변환이 언제나 정밀도까지 보존한다는 뜻은 아닙니다.
큰 long을 double로 변환하면 double의 유효 자릿수 한계로 정밀도를 잃을 수 있습니다.

`(int)measured`처럼 타입을 괄호 안에 쓰면 명시적 캐스트입니다.
예제의 3.9는 int 3이 됩니다.
이 동작은 반올림이 아니라 소수 부분을 0 방향으로 버리는 것입니다.
범위 밖의 값과 NaN 등에는 추가 규칙이 있으므로 캐스트를 입력 검증 대신 쓰지 마세요.

문자열 `"42"`를 숫자로 바꾸는 일은 텍스트 문법을 해석하는 파싱입니다.
`(int)"42"`는 올바른 변환 코드가 아닙니다.
숫자끼리의 캐스트와 문자열 파싱은 목적이 다릅니다.

## 4. 실습 2 — TryParse의 두 결과

[Program.cs](Program.cs)는 "42"와 "사십이"를 각각 변환합니다.

```csharp
bool parsed = int.TryParse(
    input,
    NumberStyles.Integer,
    CultureInfo.InvariantCulture,
    out int number);
```

1. TryParse는 문자열이 int 범위의 정수인지 검사합니다.
2. 성공 여부는 bool 반환값으로 받습니다.
3. 성공한 정수 값은 out 변수 number로 받습니다.
4. 실패하면 false이며 이 오버로드의 number에는 0이 들어갑니다.

실패의 0과 유효한 입력 "0"을 구분하려면 bool 결과를 반드시 검사해야 합니다.
out은 메서드가 추가 값을 돌려주는 문법이며 08단원에서 자세히 배웁니다.
Parse는 잘못된 형식이나 범위 초과에서 예외를 던질 수 있습니다.
실패가 자연스러운 사용자 입력 처리에는 TryParse가 흐름을 표현하기 편합니다.

문화권에 따라 숫자 표기가 달라질 수 있어 예제는 불변 문화권을 명시합니다.
파싱 성공과 업무 규칙 통과는 별개입니다.
"-5"는 int로 유효하지만 나이로는 부적절하므로 범위 검사도 필요합니다.

## 5. 실습 3 — &&와 &

bool에서 `&&`는 왼쪽이 false이면 오른쪽을 실행하지 않습니다.
전체 AND 결과가 이미 false로 확정되기 때문이며, 이를 단락 평가라고 합니다.
`||`는 왼쪽이 true이면 오른쪽을 생략합니다.
bool의 `&`와 `|`는 양쪽을 모두 평가합니다.

[Program.cs](Program.cs)는 오른쪽의 `++checks`로 평가 횟수를 관찰합니다.

1. left는 false입니다.
2. && 식은 오른쪽을 건너뛰어 checks가 0입니다.
3. & 식은 오른쪽도 실행하여 checks가 1입니다.
4. 두 식의 bool 결과 자체는 모두 false입니다.

이 예제는 차이를 관찰하기 위한 것이며 실무에서는 부수 효과가 많은 조건식을 피하세요.
정수에 &와 |를 사용하면 각 비트를 연산하는 다른 의미가 있습니다.

```csharp
string? text = null;
bool hasText = text != null && text.Length > 0;
```

null일 때는 오른쪽 Length를 읽지 않으므로 안전합니다.
&&를 &로 바꾸면 오른쪽도 평가되어 null 접근 문제가 생길 수 있습니다.

## 6. 실습 4 — checked와 범위 초과

정수의 최대값에 1을 더한 수학적 결과는 해당 정수 타입에 담을 수 없습니다.
이 현상을 오버플로라고 합니다.
checked는 지정된 정수 연산과 변환에서 범위 초과를 검출하도록 합니다.

```csharp
int largest = int.MaxValue;
int result = checked(largest + 1);
```

예제에서는 발생한 OverflowException을 catch하여 메시지로 보여줍니다.
try/catch의 세부 규칙은 예외 처리 단원에서 배웁니다.
지금은 실패를 눈에 보이게 알려주는 흐름에 집중하세요.

실행 중 정수 계산은 컴파일러 설정과 checked 문맥의 영향을 받습니다.
상수 계산은 컴파일 시 오류가 발생할 수도 있습니다.
모든 정수 계산이 항상 감기거나 항상 예외를 던진다고 일반화하지 마세요.

`(long)unitPrice * quantity`는 곱하기 전에 넓은 타입으로 계산합니다.
`(long)(unitPrice * quantity)`는 int 계산이 끝난 뒤 바꾸므로 이미 발생한 오버플로를 해결하지 못합니다.

## 7. 자주 하는 실수

- `double average = sum / count;`: 둘 다 int이면 나누기 전에 한쪽을 변환하세요.
- TryParse의 bool 무시: 변환 실패를 정상 0으로 오인할 수 있습니다.
- 반올림을 원하면서 int 캐스트 사용: 원하는 반올림 규칙을 명시적으로 선택하세요.
- 계산 뒤에 long으로 변환: 피연산자를 먼저 변환해야 계산 범위가 넓어집니다.
- `=`와 `==` 혼동: 대입과 비교를 구별하세요.

## 기본 실행의 예상 출력

[expected-output.txt](expected-output.txt)와 비교하세요.

```text
정수 나눗셈: 3
실수 나눗셈: 3.5
나머지: 1
변환 성공: True, 값: 42
잘못된 입력: False, 결과: 0
&& 결과: False, 검사 횟수: 0
& 결과: False, 검사 횟수: 1
checked: 정수 범위를 초과했습니다.
명시적 변환: 3
```

## 직접 풀어보기

1. 기초: 135분을 나눗셈과 나머지로 2시간 15분으로 표시하세요.
2. 응용: 문자열 나이를 TryParse로 읽고 0~150일 때만 유효하다고 출력하세요.
3. 도전: int 단가 1,500,000,000과 수량 2의 정확한 총액을 long으로 계산하세요.

<details>
<summary>해설과 실행 가능한 정답</summary>

시간은 / 60, 분은 % 60입니다.
나이 검사에서는 TryParse 성공과 범위를 &&로 연결합니다.
큰 금액은 곱하기 전 피연산자를 long으로 바꿔 3000000000을 얻습니다.
[ExerciseSolutions.cs](ExerciseSolutions.cs)를 읽고 `--answers`로 실행하세요.

</details>

## 복습 질문

### 1. double result = 7 / 2는 왜 3인가요?

<details>
<summary>정답</summary>

오른쪽 int 나눗셈이 먼저 수행되어 3이 된 뒤 double로 바뀌기 때문입니다.

</details>

### 2. &&와 &의 결과가 같으면 실행도 같나요?

<details>
<summary>정답</summary>

아닙니다. &&는 필요 없으면 오른쪽을 생략하고 &는 양쪽을 평가합니다.

</details>

### 3. TryParse 성공이면 나이도 유효한가요?

<details>
<summary>정답</summary>

아닙니다. 숫자 형식과 타입 범위 외에 나이에 맞는 업무 규칙도 검사해야 합니다.

</details>

## 공식 자료

- [Microsoft Learn: 산술 연산자](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/arithmetic-operators)
- [Microsoft Learn: 논리 연산과 단락 평가](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/boolean-logical-operators)
- [Microsoft Learn: Int32.TryParse](https://learn.microsoft.com/en-us/dotnet/api/system.int32.tryparse?view=net-10.0)
