# C# 문법 아틀라스 · 한국어 학습 노트

**개념을 읽고 → 결과를 예상하고 → Visual Studio에서 실행하고 → 값을 바꾸어 확인하는 C# 학습 자료입니다.**

입문부터 중급까지 24개 단원, 실행 가능한 예제, 단원마다 3개의 연습문제와 해설, 성적 관리 종합 프로젝트를 담았습니다.
설명은 한국어이며, 코드 식별자는 일반적인 C# 관례를 익힐 수 있도록 영어를 사용합니다.
각 프로젝트는 독립적으로 실행됩니다. 한 파일에 여러 단원의 `Main`을 붙여 넣을 필요가 없습니다.

## 바로 시작하기

1. `CSharpGrammarAtlas.sln`을 Visual Studio 2026으로 엽니다.
2. 솔루션 탐색기에서 `chapters` → `Chapter01`을 펼칩니다.
3. `README.md`에서 개념을 읽고 `Program.cs`에서 같은 예제를 확인합니다.
4. `Chapter01`을 오른쪽 클릭하여 **시작 프로젝트로 설정**합니다.
5. `Ctrl + F5`로 실행합니다. 출력은 단원의 `expected-output.txt`와 비교할 수 있습니다.
6. 다음 단원으로 넘어갈 때 시작 프로젝트를 해당 `ChapterNN`으로 바꿉니다.

문서는 GitHub에서 목차·코드·표·접힌 해설을 포함한 형태로 읽을 수 있습니다.
Visual Studio에서는 `README.md`를 열어 원문 또는 제공되는 미리 보기로 읽으면 됩니다.
화면 메뉴가 낯설다면 [Visual Studio 실행·디버깅 안내](docs/VisualStudioGuide.md)를 먼저 읽으세요.

### 터미널에서 실행

아래 명령은 **이 README가 있는 저장소 루트**에서 실행합니다.

```powershell
dotnet build CSharpGrammarAtlas.sln
dotnet run --project chapters/01-ProgramBasics/Chapter01.csproj
dotnet run --project chapters/01-ProgramBasics/Chapter01.csproj -- --answers
```

첫 명령은 전체 예제를 컴파일합니다. 둘째는 본문 예제, 셋째는 연습문제 해설 코드를 실행합니다.
`--` 뒤의 값은 `dotnet` 옵션이 아니라 **작성한 프로그램이 받는 인수**입니다.

## 학습 환경과 범위

| 항목 | 설정 |
|---|---|
| IDE | Visual Studio 2026, C# 콘솔 프로젝트를 사용할 수 있는 .NET 개발 구성 |
| 대상 프레임워크 | .NET 10 (`net10.0`) |
| 언어 버전 | C# 14.0으로 명시적으로 고정 |
| SDK | .NET 10 SDK; `global.json`은 설치된 10.0 기능 밴드 중 최신을 허용 |
| 운영체제 | 예제는 Windows·macOS·Linux의 .NET CLI에서 실행 가능; VS 안내는 Windows 기준 |
| 외부 패키지 | 없음. .NET에 포함된 기본 라이브러리만 사용 |
| Nullable | 활성화. 널 가능성을 코드에서 드러내고 확인 |

기초 단원은 오래전부터 쓰인 핵심 문법에 집중합니다. 현대적인 표현이 등장하면 필요한 버전을 함께 설명합니다.
24장은 C# 12~14에서 추가된 일부 문법을 비교하는 단원입니다. 미리 보기 언어 버전은 사용하지 않습니다.
Unity·ASP.NET Core·WPF의 프레임워크별 개발 방법과 `unsafe` 포인터 등 전문 분야 전체를 다루는 언어 명세서는 아닙니다.

.NET SDK, C# 언어, Visual Studio는 서로 다른 제품/버전입니다.
`.NET 10`과 기본 `C# 14`의 대응은 [Microsoft 언어 버전 문서](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-versioning),
SDK와 Visual Studio의 호환성은 [공식 호환성 안내](https://learn.microsoft.com/en-us/dotnet/core/porting/versioning-sdk-msbuild-vs)를 참고하세요.

## 전체 목차

### 1부 · 프로그램의 기본 재료

| 단원 | 주요 내용 | 실습 프로젝트 |
|---|---|---|
| [01. 프로그램의 구조](chapters/01-ProgramBasics/README.md) | 문장, 주석, 출력, 진입점, 컴파일과 실행 | `Chapter01` |
| [02. 변수와 자료형](chapters/02-VariablesTypes/README.md) | 정수·실수·문자·논리, 선언, 초기화, `var`, 상수 | `Chapter02` |
| [03. 연산자와 형 변환](chapters/03-OperatorsConversions/README.md) | 산술·비교·논리, 캐스팅, `TryParse`, 오버플로 | `Chapter03` |
| [04. 문자열](chapters/04-Strings/README.md) | 보간, 이스케이프, 가공, 비교, 불변성 | `Chapter04` |
| [05. 조건문](chapters/05-Conditions/README.md) | `if`, `else`, `switch`, 조건식 | `Chapter05` |
| [06. 반복문](chapters/06-Loops/README.md) | `for`, `while`, `do`, `foreach`, `break`, `continue` | `Chapter06` |
| [07. 배열](chapters/07-Arrays/README.md) | 인덱스, 순회, 다차원·가변 배열 | `Chapter07` |
| [08. 메서드](chapters/08-Methods/README.md) | 매개변수, 반환, 오버로드, `ref`·`out`·`in` | `Chapter08` |

### 2부 · 데이터를 모델링하고 안전하게 다루기

| 단원 | 주요 내용 | 실습 프로젝트 |
|---|---|---|
| [09. 값 형식과 참조 형식](chapters/09-ValueReference/README.md) | 복사, 별칭, `struct`, 참조 전달 | `Chapter09` |
| [10. 클래스와 프로퍼티](chapters/10-ClassesProperties/README.md) | 객체, 생성자, 접근 제한, 캡슐화, `static` | `Chapter10` |
| [11. 상속과 다형성](chapters/11-InheritancePolymorphism/README.md) | `abstract`, `virtual`, `override`, `base` | `Chapter11` |
| [12. 인터페이스와 합성](chapters/12-InterfacesComposition/README.md) | 계약, 구현 교체, 역할 분리 | `Chapter12` |
| [13. 제네릭](chapters/13-Generics/README.md) | 형식 매개변수, 제네릭 메서드, 제약 조건 | `Chapter13` |
| [14. 컬렉션](chapters/14-Collections/README.md) | `List`, `Dictionary`, `HashSet`, `Queue`, `Stack` | `Chapter14` |
| [15. 예외와 자원 관리](chapters/15-ExceptionsResources/README.md) | `try`, `catch`, `finally`, `throw`, `using` | `Chapter15` |
| [16. Nullable과 null 처리](chapters/16-Nullable/README.md) | `T?`, 널 분석, `?.`, `??`, `??=`, `!` | `Chapter16` |

### 3부 · 실무에서 자주 만나는 표현

| 단원 | 주요 내용 | 실습 프로젝트 |
|---|---|---|
| [17. 델리게이트와 람다](chapters/17-DelegatesLambdas/README.md) | `Action`, `Func`, 조건과 동작 전달, 캡처 | `Chapter17` |
| [18. 이벤트](chapters/18-Events/README.md) | 발행자·구독자, `event`, 연결과 해제 | `Chapter18` |
| [19. LINQ](chapters/19-Linq/README.md) | 필터, 변환, 정렬, 그룹, 조인, 지연 실행 | `Chapter19` |
| [20. 레코드와 패턴 매칭](chapters/20-RecordsPatterns/README.md) | 값 비교, `with`, 속성·관계·리스트 패턴 | `Chapter20` |
| [21. 파일과 JSON](chapters/21-FilesJson/README.md) | 경로, 파일 읽기/쓰기, 직렬화 | `Chapter21` |
| [22. 비동기](chapters/22-AsyncAwait/README.md) | `Task`, `async`, `await`, 병행 대기, 취소 | `Chapter22` |
| [23. 확장 문법](chapters/23-AdvancedSyntax/README.md) | 튜플, 분해, 확장 메서드, 인덱서, `yield` | `Chapter23` |
| [24. 현대 C# 문법](chapters/24-ModernCSharp/README.md) | 컬렉션 식, 기본 생성자, `params` 컬렉션, C# 14 | `Chapter24` |

## 종합 실습 · 성적 관리

[성적 관리 프로젝트 해설](projects/Gradebook/README.md)에서는 여러 단원의 내용을 하나로 연결합니다.
학생 추가·검색·삭제, 점수 정렬·평균·등급 계산, JSON 저장·불러오기를 직접 사용할 수 있습니다.

```powershell
# 정해진 데이터로 실행하는 데모
dotnet run --project projects/Gradebook/Gradebook.csproj

# 직접 메뉴를 선택하는 모드
dotnet run --project projects/Gradebook/Gradebook.csproj -- --interactive

# 경계값, 중복 이름, JSON 왕복, 오류 시 상태 보존 검증
dotnet run --project projects/Gradebook/Gradebook.csproj -- --self-test
```

대화형 모드에서 저장을 선택하면 **실행 시 작업 폴더**의 `data/students.json`을 사용합니다.
이 데이터 폴더는 Git에서 제외됩니다. 기본 데모는 파일을 만들지 않습니다.
21장 파일 예제와 자체 검증은 각 실행이 만든 고유한 임시 폴더만 사용하고 정리합니다.

## 한 단원을 공부하는 방법

1. **개념부터 읽기**: 문법 이름보다 해결하려는 문제를 이해합니다.
2. **예상 결과 적기**: 실행 버튼을 누르기 전에 변수의 값과 출력 순서를 써 봅니다.
3. **예제 실행**: 결과가 예상과 다르면 해당 문장에 중단점을 걸어 추적합니다.
4. **값 하나 바꾸기**: 0, 빈 문자열, 빈 목록, 경계값 등으로 바꿔 이유를 설명합니다.
5. **연습문제 풀기**: 해설을 펼치기 전 새 메서드나 별도 연습 프로젝트에서 구현합니다.
6. **해설과 비교**: 정답 코드뿐 아니라 왜 그 자료형과 분기를 선택했는지 확인합니다.

`Program.cs`는 기본 예제, `ExerciseSolutions.cs`는 연습문제 해설입니다.
해설을 실행할 때는 `--answers`를 전달합니다. 22장 해설은 비동기 메서드를 사용합니다.
학습 중 예제를 수정하면 원래 `expected-output.txt`와 달라질 수 있습니다. 이때 검증 실패는 자연스러운 결과입니다.

## 부록과 문제 해결

- [Visual Studio에서 실행하고 디버깅하기](docs/VisualStudioGuide.md)
- [문법 빠른 찾기와 혼동하기 쉬운 개념](docs/QuickReference.md)
- [4주 학습 순서와 복습 기준](docs/StudyPlan.md)
- [버전·파일 구성·검증 방식](docs/RepositoryGuide.md)

## 전체 예제 검증

PowerShell에서 실행합니다.

```powershell
pwsh -File scripts/Verify.ps1
```

PowerShell 7이 없다면 Windows PowerShell에서 `./scripts/Verify.ps1`을 실행할 수 있습니다.
실행 정책이 차단한다면 전역 보안 설정을 바꾸지 말고, 위의 `dotnet build`와 개별 `dotnet run` 명령으로 확인하세요.

검증 스크립트는 전체를 Release로 빌드하고, 24개 기본 예제 및 성적 관리 데모의 표준 출력을 예상 파일과 비교합니다.
24개 해설 실행의 정상 종료와 성적 관리 자체 검증도 확인합니다.
GitHub Actions에도 같은 검증 명령이 설정되어 있습니다. 로컬 검증과 GitHub에서의 실행 결과는 별개입니다.

## 참고 자료

이 저장소의 설명과 예제는 학습용으로 새로 구성했습니다. 세부 규칙은 각 단원 끝에 연결한 Microsoft 문서를 확인하세요.
[C# 공식 문서](https://learn.microsoft.com/en-us/dotnet/csharp/)와
[C# 언어 참조](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/)에서 더 깊게 찾아볼 수 있습니다.
