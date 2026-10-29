using System.Globalization;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 예제 1: 배열의 인덱스는 0부터 Length - 1까지입니다.
int[] scores = { 80, 90, 100 };
int sum = 0;
for (int i = 0; i < scores.Length; i++)
{
    sum += scores[i];
}
double average = (double)sum / scores.Length;
Console.WriteLine($"첫 점수: {scores[0]}, 마지막 점수: {scores[^1]}");
Console.WriteLine($"합: {sum}, 평균: {average:F1}");

// 예제 2: 새 배열의 요소는 타입의 기본값으로 초기화됩니다.
int[] empty = new int[3];
Console.WriteLine($"기본값: {string.Join(", ", empty)}");
int[] copied = (int[])scores.Clone();
copied[0] = 50;
Console.WriteLine($"원본: {scores[0]}, 복사본: {copied[0]}");

// 예제 3: 직사각형 배열의 각 차원 길이를 따로 읽습니다.
int[,] grid = { { 1, 2, 3 }, { 4, 5, 6 } };
Console.WriteLine($"격자: {grid.GetLength(0)}행 {grid.GetLength(1)}열, 전체 {grid.Length}개");
Console.WriteLine($"격자 [1,2]: {grid[1, 2]}");

// 예제 4: 가변 배열은 각 행이 별개의 배열입니다.
int[][] rows = { new[] { 10, 20 }, new[] { 30, 40, 50 } };
Console.WriteLine($"가변 배열 길이: {rows[0].Length}, {rows[1].Length}");
Console.WriteLine($"가변 배열 [1][2]: {rows[1][2]}");

// 범위 표현식으로 배열 일부를 새 배열로 복사할 수 있습니다(C# 8+).
int[] lastTwo = scores[1..];
Console.WriteLine($"부분 배열: {string.Join(", ", lastTwo)}");
