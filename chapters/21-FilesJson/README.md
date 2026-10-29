# 21장 · 파일 입출력과 JSON

[← 20장 레코드와 패턴](../20-RecordsPatterns/README.md) · [전체 목차](../../README.md) · [22장 async와 await →](../22-AsyncAwait/README.md)

## 먼저 알아둘 내용

문자열, 예외 처리, using, 클래스와 레코드, nullable을 복습하세요.
파일 시스템에 데이터를 저장하면 프로그램이 종료된 뒤에도 내용을 다시 읽을 수 있습니다.
이 장은 외부 패키지 없이 System.IO와 System.Text.Json만 사용합니다.
[Program.cs](Program.cs)는 실행 전용 임시 폴더를 만들고 모든 작업 후 그 폴더만 정리합니다.

## 학습 목표

- Path로 경로를 조합하고 파일명과 전체 경로를 구분합니다.
- 문자열 전체 읽기와 스트림 기반 읽기의 차이를 이해합니다.
- using이 파일 핸들의 수명을 관리하는 이유를 설명합니다.
- 객체를 JSON으로 직렬화하고 다시 객체로 복원합니다.
- 잘못된 JSON, JSON null, 업무 데이터 검증을 구분합니다.

## 1. 경로는 문자열이지만 운영체제 규칙을 따른다

파일 위치는 작업 디렉터리, 절대 경로, 상대 경로의 영향을 받습니다.
실행 파일 옆에 저장된다고 생각하고 상대 경로만 쓰면 Visual Studio와 터미널에서 다른 위치를 사용할 수 있습니다.
따라서 예제는 OS의 임시 위치 아래에 자신만의 디렉터리를 먼저 만듭니다.

~~~csharp
DirectoryInfo ownedDirectory = Directory.CreateTempSubdirectory("csharp-atlas-21-");
string textPath = Path.Combine(ownedDirectory.FullName, "notes.txt");
~~~

이 디렉터리 이름은 실행마다 다르지만 화면에는 실제 절대 경로를 출력하지 않습니다.
Path.Combine은 운영체제에 맞는 경로 구분자를 사용해 조합합니다.
직접 "/" 또는 "\\"를 이어 붙이는 것보다 의도가 명확합니다.

Path.Combine이 사용자 입력을 안전하게 제한해 주는 검증 함수는 아닙니다.
외부에서 받은 절대 경로, .. 같은 상위 경로 요소를 허용할지는 별도로 결정해야 합니다.
이 예제에서는 파일명을 코드 안의 상수로 고정하고 외부 경로 입력을 받지 않습니다.

## 2. 파일 핸들은 왜 닫아야 할까?

파일을 열면 운영체제의 파일 핸들과 버퍼 같은 리소스를 사용합니다.
GC는 관리 객체의 메모리를 다루지만 파일을 원하는 시점에 닫는 계약을 대신하지 않습니다.
IDisposable과 using은 사용이 끝나는 범위를 명확하게 만들어 줍니다.

~~~csharp
using (StreamWriter writer = new(textPath, false, new UTF8Encoding(false)))
{
    writer.WriteLine("C#");
    writer.WriteLine("JSON");
}
~~~

using 블록이 끝나면 Dispose가 호출됩니다.
본문에서 예외가 발생해도 정리 경로를 거칩니다.
StreamWriter는 아직 메모리 버퍼에 남은 내용도 파일에 기록합니다.
읽기 전에 쓰기 스트림을 닫아야 예제의 내용을 확실하게 읽을 수 있습니다.

false 인수는 append가 아니라 덮어쓰기 방식이라는 뜻입니다.
같은 경로의 기존 파일이 있으면 내용이 바뀌므로 실제 프로그램에서는 저장 정책을 정해야 합니다.
여기서는 새로 만든 실행 전용 디렉터리만 사용합니다.

## 3. 실행 예제 A: 두 줄 쓰고 읽기

~~~csharp
using (StreamReader reader = new(textPath, Encoding.UTF8))
{
    Console.WriteLine($"first-line={reader.ReadLine()}");
    Console.WriteLine($"second-line={reader.ReadLine()}");
}
~~~

ReadLine은 줄 끝 문자를 제거한 문자열을 반환하고 더 읽을 줄이 없으면 null을 반환합니다.
빈 줄은 빈 문자열이며 파일의 끝인 null과 다릅니다.
텍스트를 저장할 때와 읽을 때 같은 인코딩을 사용해야 글자가 일관되게 해석됩니다.
예제는 UTF-8을 명시하고 BOM 없는 쓰기를 사용합니다.

작은 파일은 File.WriteAllText와 File.ReadAllText가 간단합니다.
파일 전체가 문자열 하나로 메모리에 올라간다는 점을 고려하세요.
큰 파일을 순차 처리하려면 스트림 또는 줄 단위 처리가 더 적절할 수 있습니다.
비동기 파일 API는 다음 장의 Task와 await를 이해한 뒤 적용할 수 있습니다.

## 4. 실행 예제 B: 객체와 JSON의 왕복

~~~csharp
StudyProfile profile = new("Ada", [1, 2, 3]);
string json = JsonSerializer.Serialize(profile);
File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
~~~

직렬화는 객체의 데이터를 저장·전달 가능한 표현으로 바꾸는 과정입니다.
JSON은 객체, 배열, 문자열, 숫자, 불리언, null을 표현할 수 있는 텍스트 형식입니다.
C#의 메서드나 객체 참조 관계를 그대로 저장하는 방식은 아닙니다.
예제의 공개 프로퍼티 Name과 CompletedChapters가 JSON에 나타납니다.

~~~csharp
StudyProfile loaded = JsonSerializer.Deserialize<StudyProfile>(File.ReadAllText(jsonPath))
    ?? throw new InvalidDataException("Profile JSON must not be null.");
~~~

역직렬화는 지정한 C# 형식으로 데이터를 복원합니다.
제네릭 인수 StudyProfile이 결과의 목표 형식입니다.
JSON 텍스트가 "null"이면 정상적인 JSON이어도 역직렬화 결과는 null일 수 있습니다.
따라서 ?? throw로 이 예제가 요구하는 “프로필 객체가 반드시 있음”이라는 조건을 표현합니다.

프로퍼티의 nullable 주석만으로 모든 외부 데이터의 유효성을 보장한다고 생각해서는 안 됩니다.
필수 값 존재 여부, 빈 이름 허용 여부, 챕터 번호 범위 등 업무 규칙은 별도 검증 대상입니다.
이 예제의 데이터는 프로그램 내부에서 직접 만들었으므로 그 규칙을 만족합니다.

## 5. 실행 예제 C: 정리 범위를 소유권에 맞추기

모든 파일 작업은 try 안에 있으며 finally에서 ownedDirectory.Delete(recursive: true)를 호출합니다.
이 경로는 바로 앞에서 OS가 생성한 이번 실행 전용 디렉터리입니다.
사용자가 지정한 문서 폴더나 임시 루트 전체를 정리하지 않습니다.
파일 예제를 반복 실행해도 임시 자료가 남지 않도록 하는 구성입니다.

실제 파일 정리는 권한 부족이나 외부 프로세스의 파일 잠금 때문에 실패할 수 있습니다.
finally에 넣었다고 삭제의 성공까지 보장되는 것은 아닙니다.
정리 실패를 숨기지 않고 원인을 관찰할 수 있도록 프로그램 정책을 정하세요.

## 6. 오류를 분류해서 이해하기

파일이 없으면 FileNotFoundException, 접근 권한 문제가 있으면 UnauthorizedAccessException 같은 예외가 발생할 수 있습니다.
JSON 구문이 잘못되었거나 목표 형식으로 읽을 수 없으면 JsonException이 발생할 수 있습니다.
반면 JSON null은 구문 오류가 아니며 정상 결과가 null인 상황입니다.
예외를 한꺼번에 잡고 “성공”처럼 처리하면 어떤 문제가 있었는지 알기 어려워집니다.

정답 예제는 잘못된 JSON을 의도적으로 읽고 JsonException만 처리합니다.
사용자에게 보여 줄 메시지와 디버깅에 필요한 세부 정보의 역할도 구분해야 합니다.
파일 경로 등 환경별 값에 의존하지 않도록 이 장의 출력은 고정 문자열로 구성했습니다.

## 실행과 정확한 기본 출력

~~~powershell
dotnet run --project chapters/21-FilesJson/Chapter21.csproj
dotnet run --project chapters/21-FilesJson/Chapter21.csproj -- --answers
~~~

~~~text
first-line=C#
second-line=JSON
json={"Name":"Ada","CompletedChapters":[1,2,3]}
name=Ada
completed=1,2,3
file=profile.json
cleaned=True
~~~

## 자주 하는 실수와 이유

| 실수 | 영향 | 해결 방향 |
| --- | --- | --- |
| 스트림을 열어 놓고 정리하지 않기 | 파일 잠금과 버퍼 문제가 남을 수 있습니다. | using으로 범위를 정합니다. |
| 역직렬화 성공이면 업무적으로 유효하다고 믿기 | 빈 값과 잘못된 범위가 남을 수 있습니다. | 복원 후 업무 규칙을 검증합니다. |
| ReadLine의 null과 빈 문자열을 동일시하기 | 파일 끝과 빈 줄을 구분하지 못합니다. | null을 먼저 검사합니다. |
| JSON 프로퍼티 이름 대소문자가 항상 무시된다고 믿기 | 기본 옵션에 따라 매칭이 달라집니다. | JsonSerializerOptions를 명시합니다. |
| 상대 경로를 실행 파일 기준으로 가정하기 | 작업 디렉터리와 달라질 수 있습니다. | 기준 디렉터리를 명확히 정합니다. |

## 연습문제

### 1단계 · 작은 파일 왕복

전용 임시 디렉터리에 practice.txt를 만들고 practice를 저장한 뒤 읽으세요.

<details>
<summary>정답과 해설</summary>

ExerciseSolutions.cs의 첫 번째 예제를 실행하세요.
Path.Combine으로 파일 경로를 만들고 WriteAllText와 ReadAllText를 사용합니다.
마지막 finally가 생성한 디렉터리만 정리합니다.

</details>

### 2단계 · 다른 프로필 복원

이름 Ben, 완료 단원 [4, 5]인 프로필을 JSON 문자열로 바꾼 뒤 다시 복원하세요.
파일 저장 없이도 직렬화 왕복이 가능한지 확인하세요.

<details>
<summary>정답과 해설</summary>

~~~csharp
string json = JsonSerializer.Serialize(original);
StudyProfile restored = JsonSerializer.Deserialize<StudyProfile>(json)
    ?? throw new InvalidDataException("Expected a profile.");
~~~

JSON 변환 자체는 파일 시스템과 독립적입니다.
정답은 restored=Ben:2를 출력합니다.

</details>

### 3단계 · 구문 오류와 null

"{ invalid }"와 "null"을 각각 역직렬화해서 결과의 차이를 확인하세요.

<details>
<summary>정답과 해설</summary>

첫 문자열은 JsonException으로 처리되어 invalid-json을 출력합니다.
두 번째는 유효한 JSON null이므로 결과를 is null로 검사해 json-null=True를 출력합니다.
실행 가능한 예외 처리 전체 코드는 ExerciseSolutions.cs에 있습니다.

</details>

## 복습 Q&A

**Q. using은 파일 내용도 자동으로 지우나요?**  
A. 아닙니다. 스트림 등의 리소스를 해제합니다. 파일 삭제는 별도 작업입니다.

**Q. JSON 문자열이 유효하면 원하는 객체가 항상 나오나요?**  
A. 아닙니다. 목표 형식과의 호환성, null 여부, 업무 유효성을 각각 고려해야 합니다.

**Q. Path.Combine은 경로가 내 폴더 안에 있는지 검증하나요?**  
A. 아닙니다. 경로 조합 함수이며 외부 입력에 대한 접근 범위 검증은 별도 책임입니다.

## 공식 문서

- [파일과 스트림 I/O](https://learn.microsoft.com/dotnet/standard/io/)
- [using 문](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/using)
- [System.Text.Json 개요](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/overview)
- [JSON 역직렬화](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/deserialization)
- [Directory.CreateTempSubdirectory](https://learn.microsoft.com/dotnet/api/system.io.directory.createtempsubdirectory)
