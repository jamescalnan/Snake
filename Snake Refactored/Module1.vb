Imports System.IO
Module Module1
    Sub Main()
        Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight)
        Console.WriteLine("use arrow keys to change direction, press escape to exit, enter to pause and s for stats")
        Console.ReadKey()
        Console.Clear()
        Console.CursorVisible = False
        Dim SnakeBody As New List(Of Point)
        Dim SnakeHead, Apple, AppleOffset As New Point(3, 6)
        Dim r As New Random
        Dim Direction, SnakeLength, MinHeight, Width, Height, AppleScore As Integer
        While 1
            Console.BackgroundColor = ConsoleColor.Black
            Console.ForegroundColor = ConsoleColor.Black
            SnakeHead.Update(3, 6)
            SnakeBody.Clear()
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
                If SnakeBody.Contains(Apple) Or SnakeBody.Contains(AppleOffset) Then
                    Do
                        Apple.Update(r.Next(2, Width), r.Next(MinHeight + 1, Height))
                    Loop Until Not SnakeBody.Contains(Apple) And Apple.X Mod 2 <> 0
                    Apple.Print("XX", ConsoleColor.Red)
                    AppleOffset.Update(Apple.X + 1, Apple.Y)
                    SnakeLength += 1
                    AppleScore += 1
                    PrintScore(AppleScore)
                End If
                For i = 0 To 1
                    If Console.KeyAvailable Then
                        Dim x, y As Integer
                        x = Console.CursorLeft
                        y = Console.CursorTop
                        Dim temp As New Point(x, y)
                        Dim temp1 As New Point(x - 1, y)
                        If Not SnakeBody.Contains(temp) Then
                            Console.BackgroundColor = ConsoleColor.Black
                        ElseIf SnakeBody.Contains(temp) Or SnakeBody.Contains(temp1) Then
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
                    'If Count > 5 Then Direction = GetComputerDir(SnakeBody, SnakeHead, Apple, Width, Height, MinHeight)
                    Threading.Thread.Sleep(2)
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
                If SnakeBody.Contains(SnakeHead) Or Not SnakeHead.WithinLimits(0, Width, Height, MinHeight) Then
                    SnakeBody.Reverse()
                    For Each position In SnakeBody
                        position.Print("XX", ConsoleColor.DarkRed)
                        Threading.Thread.Sleep(5)
                    Next
                    Exit While
                End If 'Should the snake be dead?
                SnakeHead.Print("XX", ConsoleColor.Green)
                SnakeBody.Add(New Point(SnakeHead.X, SnakeHead.Y))
                If SnakeBody.Count - 1 > SnakeLength Then
                    SnakeBody(0).Print("  ", ConsoleColor.Black)
                    SnakeBody.RemoveAt(0)
                End If
                time_elapsed(Start)
                'Count += 1
            End While
            If AppleScore <> 0 Then RecordScore(AppleScore)
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
            Dim arr() As String = {"Stats retrieved from the text file:", $"Number of Scores recorded: {Scores.Count}", $"Average Score: {Math.Ceiling(Scores.Average)}", $"Highest Score: {Scores.Max}", $"Lowest Score: {Scores.Min}", $"Most common score: {Scores.GroupBy(Function(n) n).OrderByDescending(Function(g) g.Count).Select(Function(g) g.Key).FirstOrDefault}"}
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
    Function GetComputerDir(ByVal SnakeBody As List(Of Point), ByVal SnakeHead As Point, ByVal Apple As Point, ByVal width As Integer, ByVal height As Integer, ByVal minheight As Integer)
        Dim PrefDirection As Integer
        If SnakeHead.X > Apple.X Then
            PrefDirection = 2
        ElseIf SnakeHead.X < Apple.X Then
            PrefDirection = 4
        End If
        If SnakeHead.Y < Apple.Y Then
            PrefDirection = 1
        ElseIf SnakeHead.Y > Apple.Y Then
            PrefDirection = 3
        End If
        Dim TempSnakeHead As New Point(SnakeHead.X, SnakeHead.Y)
        If PrefDirection = 1 Then
            TempSnakeHead.Update(SnakeHead.X, SnakeHead.Y + 1)
        ElseIf PrefDirection = 2 Then
            TempSnakeHead.Update(SnakeHead.X - 2, SnakeHead.Y)
        ElseIf PrefDirection = 3 Then
            TempSnakeHead.Update(SnakeHead.X, SnakeHead.Y - 1)
        ElseIf PrefDirection = 4 Then
            TempSnakeHead.Update(SnakeHead.X + 2, SnakeHead.Y)
        End If
        If SnakeBody.Contains(TempSnakeHead) Then
            Dim dir As Integer
            If PrefDirection = 4 Or PrefDirection = 2 Then
                For i = 1 To 4
                    dir = GetNextSnakePos(i, SnakeHead, SnakeBody, width, height, minheight)
                    If dir <> -1 Then Return dir
                Next
            Else
                Dim c As Integer = 4
                For i = 1 To 4
                    dir = GetNextSnakePos(c, SnakeHead, SnakeBody, width, height, minheight)
                    If dir <> -1 Then Return dir
                    c -= 1
                Next
            End If
        End If
        Return PrefDirection
    End Function
    Function GetNextSnakePos(ByVal dir As Integer, ByVal point As Point, ByVal list As List(Of Point), ByVal width As Integer, ByVal height As Integer, ByVal minheight As Integer)
        Dim temp As New Point(0, 0)
        If dir = 1 Then
            temp.Update(point.X, point.Y + 1)
            If Not list.Contains(temp) And temp.WithinLimits(1, width - 1, height - 1, minheight) Then
                Return dir
            End If
        ElseIf dir = 2 Then
            temp.Update(point.X - 2, point.Y)
            If Not list.Contains(temp) And temp.WithinLimits(1, width - 1, height - 1, minheight) Then
                Return dir
            End If
        ElseIf dir = 3 Then
            temp.Update(point.X, point.Y - 1)
            If Not list.Contains(temp) And temp.WithinLimits(1, width - 1, height - 1, minheight) Then
                Return dir
            End If
        ElseIf dir = 4 Then
            temp.Update(point.X + 2, point.Y)
            If Not list.Contains(temp) And temp.WithinLimits(1, width - 1, height - 1, minheight) Then
                Return dir
            End If
        End If
        Return -1
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
    Function WithinLimits(ByVal minwidth As Integer, ByVal width As Integer, ByVal height As Integer, ByVal minheight As Integer)
        If Me.X >= minwidth And Me.X <= width And Me.Y >= minheight + 1 And Me.Y <= height Then Return True
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
