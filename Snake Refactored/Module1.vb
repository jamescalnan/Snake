Imports System.IO
Module Module1
    Sub Main()
        Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight)
        Console.WriteLine("use arrow keys to change direction, press escape to exit, enter to pause and s for stats")
        Console.ReadKey()
        Console.Clear()
        Console.CursorVisible = False
        Dim arr As New List(Of Point)
        Dim SnakeHead, Apple, AppleOffset As New Point(3, 6)
        Dim Scores As New List(Of Integer)
        Dim r As New Random
        Dim Direction, SnakeLength, MinHeight, Width, Height, AppleScore As Integer
        Dim c As Integer = 0
        While 1
            Console.BackgroundColor = ConsoleColor.Black
            Console.ForegroundColor = ConsoleColor.Black
            SnakeHead.Update(3, 6)
            arr.Clear()
            AppleScore = 0
            Width = Console.WindowWidth - 1
            Height = Console.WindowHeight - 1
            MinHeight = 3
            Console.BackgroundColor = ConsoleColor.White
            For i = 0 To Width
                Console.SetCursorPosition(i, MinHeight)
                Console.Write(" ")
            Next
            Do
                Apple.Update(r.Next(1, Width), r.Next(MinHeight + 1, Height))
            Loop Until Apple.X Mod 2 = 0
            AppleOffset.Update(Apple.X + 1, Apple.Y)
            AppleOffset.Print("XX", ConsoleColor.Red)
            Direction = 4
            SnakeLength = 6
            PrintScore(AppleScore)
            Dim Start As Stopwatch = Stopwatch.StartNew
            While 1
                If arr.Contains(Apple) Or arr.Contains(AppleOffset) Then
                    Do
                        Apple.Update(r.Next(2, Width), r.Next(MinHeight + 1, Height))
                    Loop Until Not arr.Contains(Apple) And Apple.X Mod 2 <> 0
                    Apple.Print("XX", ConsoleColor.Red)
                    AppleOffset.Update(Apple.X + 1, Apple.Y)
                    SnakeLength += 1
                    AppleScore += 1
                    PrintScore(AppleScore)
                End If
                For i = 0 To 15
                    If Console.KeyAvailable Then
                        Dim x, y As Integer
                        x = Console.CursorLeft
                        y = Console.CursorTop
                        Dim temp As New Point(x, y)
                        Dim temp1 As New Point(x - 1, y)
                        If Not arr.Contains(temp) Then
                            Console.BackgroundColor = ConsoleColor.Black
                        ElseIf arr.Contains(temp) Or arr.Contains(temp1) Then
                            Console.BackgroundColor = ConsoleColor.Green
                        End If
                        Dim key = Console.ReadKey
                        Select Case key.Key.ToString
                            Case "Enter"
                                SnakeHead.Print("XX", ConsoleColor.DarkGreen)
                                Console.BackgroundColor = ConsoleColor.Black
                                Console.ForegroundColor = ConsoleColor.Black
                                Console.ReadKey()
                                SnakeHead.Print("XX", ConsoleColor.Green)
                            Case "Escape"
                                End
                            Case "S"
                                ShowStats()
                                Exit While
                            Case "DownArrow"
                                If Direction <> 3 Then Direction = 1
                                Exit For
                            Case "RightArrow"
                                If Direction <> 2 Then Direction = 4
                                Exit For
                            Case "UpArrow"
                                If Direction <> 1 Then Direction = 3
                                Exit For
                            Case "LeftArrow"
                                If Direction <> 4 Then Direction = 2
                                Exit For
                        End Select
                    End If
                    'If c > 10 Then Direction = GetComputerDir(arr, SnakeHead, Width, MinHeight, Height, Apple, Direction)
                    Threading.Thread.Sleep(1)
                Next
                If Direction = 1 Then
                    SnakeHead.Update(SnakeHead.X, SnakeHead.Y + 1)
                ElseIf Direction = 2 Then
                    SnakeHead.Update(SnakeHead.X - 2, SnakeHead.Y)
                ElseIf Direction = 3 Then
                    SnakeHead.Update(SnakeHead.X, SnakeHead.Y - 1)
                ElseIf Direction = 4 Then
                    SnakeHead.Update(SnakeHead.X + 2, SnakeHead.Y)
                End If
                If arr.Contains(SnakeHead) Or Not SnakeHead.WithinLimits(Width, Height, MinHeight) Then
                    arr.Reverse()
                    For Each position In arr
                        position.Print("XX", ConsoleColor.DarkRed)
                        Threading.Thread.Sleep(5)
                    Next
                    Exit While
                End If
                SnakeHead.Print("XX", ConsoleColor.Green)
                arr.Add(New Point(SnakeHead.X, SnakeHead.Y))
                If arr.Count - 1 > SnakeLength Then
                    arr(0).Print("  ", ConsoleColor.Black)
                    arr.RemoveAt(0)
                End If
                time_elapsed(Start)
            End While
            If AppleScore <> 0 Then RecordScore(AppleScore)
            Console.SetCursorPosition(0, 0)
            Console.BackgroundColor = ConsoleColor.Green
            Console.ForegroundColor = ConsoleColor.Green
            Console.ReadKey()
            Console.BackgroundColor = ConsoleColor.Black
            Console.ForegroundColor = ConsoleColor.Black
            Console.Clear()
            Console.SetCursorPosition(0, 0)
        End While
    End Sub
    Sub ShowStats()
        Dim Scores As List(Of Integer) = GetScoresInList()
        If Not IsNothing(Scores) Then
            Console.Clear()
            Console.BackgroundColor = ConsoleColor.Black
            Console.ForegroundColor = ConsoleColor.White
            Dim arr() As String = {"Stats retrieved from the text file:", $"Number of Scores recorded: {Scores.Count}", $"Average Score: {Scores.Average}", $"Highest Score: {Scores.Max}", $"Lowest Score: {Scores.Min}", $"Most common score: {Scores.GroupBy(Function(n) n).OrderByDescending(Function(g) g.Count).Select(Function(g) g.Key).FirstOrDefault}"}
            Dim count As Integer = -arr.Count / 2 - 1
            For Each message In arr
                Console.SetCursorPosition(Console.WindowWidth / 2 - message.Length / 2, Console.WindowHeight / 2 + count)
                Console.Write(message)
                count += 2
            Next
        Else
            Dim message As String = "No stats available"
            Console.SetCursorPosition(Console.WindowWidth / 2 - message.Length / 2, Console.WindowHeight / 2)
            Console.Write(message)
        End If
    End Sub
    Sub ShowMaxScore()
        Console.Clear()
        Dim Scores As List(Of Integer) = GetScoresInList()
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.White
        Dim Message As String = $"High score: {Scores.Max}"
        Console.SetCursorPosition(Console.WindowWidth / 2 - Message.Length / 2, Console.WindowHeight / 2)
        Console.Write(Message)
    End Sub
    Function GetScoresInList()
        If System.IO.File.Exists("Scores.txt") Then
            Dim Scores As New List(Of Integer)
            Using reader As StreamReader = New StreamReader("Scores.txt")
                Do Until reader.EndOfStream
                    Scores.Add(Int(reader.ReadLine))
                Loop
            End Using
            Return Scores
        End If
        Return Nothing
    End Function
    Sub DisplayScoreAverage()
        Console.Clear()
        Dim Scores As List(Of Integer) = GetScoresInList()
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.White
        Dim Message As String = $"Average score: {Scores.Average}"
        Console.SetCursorPosition(Console.WindowWidth / 2 - Message.Length / 2, Console.WindowHeight / 2)
        Console.Write(Message)
    End Sub

    Sub RecordScore(ByVal Score As Integer)
        Using writer As StreamWriter = New StreamWriter("Scores.txt", True)
            writer.WriteLine(Score)
        End Using
    End Sub

    Sub time_elapsed(ByVal start As Stopwatch)
        Console.SetCursorPosition(3, 1)
        Dim sec As Integer
        Dim minamount As Integer
        minamount = Math.Floor(start.Elapsed.TotalSeconds / 60)
        sec = Math.Floor(start.Elapsed.TotalSeconds - (60 * minamount))
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.White
        If sec < 60 And minamount < 1 Then
            Console.WriteLine($"Time elapsed: {sec} {If(sec = 1, "second", "seconds")}   ")
        Else
            If sec < 10 Then
                Console.WriteLine($"Time elapsed: {Math.Floor(minamount)}:0{sec}                           ")
            Else
                Console.WriteLine($"Time elapsed: {Math.Floor(minamount)}:{sec}                            ")
            End If
        End If
    End Sub
    Sub PrintScore(ByVal AppleScore As Integer)
        Dim Message As String = $"Current Score: {AppleScore}  "
        Console.SetCursorPosition(Console.WindowWidth / 2 - (Message.Length - 2) / 2, 1)
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.White
        Console.Write(Message)
    End Sub
    Function GetComputerDir(ByVal SnakeList As List(Of Point), ByVal SnakeHead As Point, ByVal Width As Integer, ByVal MinHeight As Integer, ByVal height As Integer, ByVal Apple As Point, ByVal Direction As Integer)
        Dim ReturnDir As Integer
        If SnakeHead.Y < Apple.Y Then
            ReturnDir = 1
            'up
        End If
        If SnakeHead.Y > Apple.Y Then
            ReturnDir = 3
            'down
        End If
        If SnakeHead.X > Apple.X Then
            ReturnDir = 2
            If Direction <> 4 Then Direction = 2

            'left
        End If
        If SnakeHead.X < Apple.X Then
            ReturnDir = 4
            If Direction <> 2 Then Direction = 4

            'right
        End If
        Dim TempSnakeHead As New Point(SnakeHead.X, SnakeHead.Y)
        If ReturnDir = 1 Then
            TempSnakeHead.Update(SnakeHead.X, SnakeHead.Y + 1)
        ElseIf ReturnDir = 2 Then
            TempSnakeHead.Update(SnakeHead.X - 2, SnakeHead.Y)
        ElseIf ReturnDir = 3 Then
            TempSnakeHead.Update(SnakeHead.X, SnakeHead.Y - 1)
        ElseIf ReturnDir = 4 Then
            TempSnakeHead.Update(SnakeHead.X + 2, SnakeHead.Y)
        End If
        TempSnakeHead.Print("XX", ConsoleColor.Blue)

        If SnakeList.Contains(TempSnakeHead) Then
            'if the nextmove will result in the snake going into itself:
            If ReturnDir = 1 Or ReturnDir = 3 Then 'if its going up or down then
                TempSnakeHead.Update(SnakeHead.X - 2, SnakeHead.Y)
                TempSnakeHead.Print("XX", ConsoleColor.Red)
                Dim temppoint As New Point(SnakeHead.X + 2, SnakeHead.Y)
                temppoint.Print("XX", ConsoleColor.Cyan)
                Console.ReadKey()

                If SnakeList.Contains(TempSnakeHead) Then
                    ReturnDir = 2
                Else
                    ReturnDir = 4
                End If
            Else

                'If ReturnDir = 2 Or ReturnDir = 4 Then
                TempSnakeHead.Update(SnakeHead.X, SnakeHead.Y + 1)
                TempSnakeHead.Print("XX", ConsoleColor.Red)
                Dim temppoint As New Point(SnakeHead.X, SnakeHead.Y - 1)
                temppoint.Print("XX", ConsoleColor.Cyan)
                If SnakeList.Contains(TempSnakeHead) Then
                    ReturnDir = 3
                Else
                    ReturnDir = 1

                End If

            End If
        End If
        Return ReturnDir
    End Function

End Module
Class Point
    Public X, Y As Integer
    Public Sub New(ByVal xpoint As Integer, ByVal ypoint As Integer)
        X = xpoint
        Y = ypoint
    End Sub
    Sub Update(ByVal _x As Integer, ByVal _y As Integer)
        X = _x
        Y = _y
    End Sub
    Function WithinLimits(ByVal width As Integer, ByVal height As Integer, ByVal minheight As Integer)
        If Me.X >= 0 And Me.X <= width And Me.Y >= minheight + 1 And Me.Y <= height Then Return True
        Return False
    End Function
    Public Function Pop(ByVal list As List(Of Point))
        Dim val As Point = list(list.Count - 1)
        list.RemoveAt(list.Count - 1)
        Return val
    End Function
    Public Sub Print(ByVal str As String, ByVal consolecolour As ConsoleColor)
        Console.BackgroundColor = consolecolour
        Console.ForegroundColor = consolecolour
        Console.SetCursorPosition(X, Y)
        Console.Write(str)
    End Sub

    Public Overrides Function Equals(obj As Object) As Boolean
        Dim cell = TryCast(obj, Point)
        Return cell IsNot Nothing AndAlso
               X = cell.X AndAlso
               Y = cell.Y
    End Function
    Public Overrides Function GetHashCode() As Integer
        Dim hashCode As Long = 1855483287
        hashCode = (hashCode * -1521134295 + X.GetHashCode()).GetHashCode()
        hashCode = (hashCode * -1521134295 + Y.GetHashCode()).GetHashCode()
        Return hashCode
    End Function
End Class
