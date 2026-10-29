public static class ExerciseSolutions
{
    public static void Run()
    {
        int number = 12;
        Console.WriteLine(number % 2 == 0 ? "짝수" : "홀수");

        int orderAmount = 25000;
        bool isMember = true;
        int shipping = orderAmount >= 30000 || isMember ? 0 : 3000;
        Console.WriteLine($"배송비: {shipping}");

        int month = 2;
        switch (month)
        {
            case 12:
            case 1:
            case 2:
                Console.WriteLine("겨울");
                break;
            case 3:
            case 4:
            case 5:
                Console.WriteLine("봄");
                break;
            case 6:
            case 7:
            case 8:
                Console.WriteLine("여름");
                break;
            case 9:
            case 10:
            case 11:
                Console.WriteLine("가을");
                break;
            default:
                Console.WriteLine("월은 1~12여야 합니다.");
                break;
        }
    }
}
