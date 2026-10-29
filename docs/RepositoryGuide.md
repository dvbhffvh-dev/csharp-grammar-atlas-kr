# 저장소 구조와 검증 방식

[전체 목차](../README.md)

```text
CSharpGrammarAtlas.sln        Visual Studio에서 여는 솔루션
Directory.Build.props        모든 프로젝트가 공유하는 C#/.NET 설정
global.json                  .NET 10 SDK 선택 규칙
NuGet.Config                 외부 패키지 공급원 없이 복원
chapters/
  01-ProgramBasics/
    Chapter01.csproj          해당 단원의 실행 프로젝트
    Program.cs                본문 예제
    ExerciseSolutions.cs      문제 해설 코드
    README.md                 한국어 설명
    expected-output.txt       본문 예제 예상 출력
  ...
  24-ModernCSharp/
projects/Gradebook/           종합 실습
docs/                        사용법과 복습 자료
scripts/Verify.ps1           전체 빌드·실행·출력 비교
.github/workflows/verify.yml GitHub Actions에서 같은 검증 실행
```

## 왜 단원마다 프로젝트가 있나요?

최상위 문장을 사용하는 소스에는 컴파일러가 진입점을 만들어 줍니다.
여러 단원의 `Program.cs`를 한 콘솔 프로젝트에 넣으면 진입점과 형식 이름이 충돌하기 쉽습니다.
독립된 프로젝트로 나누면 학습 단원을 선택해서 실행하고 해당 단원의 오류만 살펴보기 쉽습니다.
동일한 이름의 보조 클래스가 다른 단원에 있어도 프로젝트가 다르면 충돌하지 않습니다.

## 왜 공통 설정을 쓰나요?

24개 프로젝트의 언어 버전을 각각 수정하면 일부만 바뀌는 실수가 생깁니다.
상위 폴더의 `Directory.Build.props`로 대상 프레임워크, nullable, 경고 정책을 공유합니다.
`LangVersion`을 `latest`로 두지 않아 새 SDK 설치 뒤 예제의 허용 문법이 바뀌는 일을 줄입니다.
SDK 패치까지 고정하지는 않습니다. `global.json`의 `latestFeature` 정책은 설치된 안정 10.0 SDK를 선택합니다.

## 검증이 확인하는 것

1. 모든 프로젝트가 경고 없이 빌드되는지 확인합니다.
2. 24개 본문 예제와 성적 관리 데모가 정상 종료하는지 확인합니다.
3. 표준 출력이 `expected-output.txt`와 일치하는지 비교합니다. 운영체제별 줄바꿈 차이는 정규화합니다.
4. 각 단원의 `--answers` 실행이 오류 없이 끝나고 출력을 만드는지 확인합니다.
5. 성적 관리 자체 검증으로 점수 범위, 중복 이름, 정렬/평균, JSON 저장/복원을 확인합니다.

출력 비교는 예제의 회귀를 잡는 도구입니다. 설명의 모든 문장이나 모든 입력 조합을 자동으로 증명하지는 않습니다.
연습문제 해설은 정상 실행을 검사하며, 별도의 해설 출력 파일과 일대일 비교하지 않습니다.
문법 오류를 설명하는 README의 코드 조각은 의도적으로 컴파일할 수 없는 예일 수 있습니다.
실행 가능한 기준은 각 프로젝트의 `.cs` 파일입니다.

## Git과 파일 저장

`bin`, `obj`, `.vs`는 빌드 결과와 개인 IDE 상태이므로 커밋하지 않습니다.
성적 관리의 `data` 폴더도 제외합니다. 소스와 학습 문서만 공유하는 구조입니다.
JSON 예제는 실습 목적으로 파일을 쓰며 해당 코드에 저장 위치와 정리 범위를 표시했습니다.

공유 라이선스는 별도로 지정하지 않았습니다. 공개 배포와 재사용 조건을 정하려면 원하는 라이선스를 추가하세요.

## 버전 확인

```powershell
dotnet --info
dotnet --list-sdks
git status
```

첫 두 명령은 실행 환경을, 마지막 명령은 예제를 수정한 파일 목록을 보여 줍니다.
이 저장소의 원본과 실습 수정 내용을 비교하면 어느 문장을 바꿨는지 복습할 수 있습니다.
