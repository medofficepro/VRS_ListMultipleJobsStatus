Imports ListMultipleJobsStatus.Process
Imports System.Threading
Module Module1
    Private WithEvents m_timer As System.Timers.Timer
    Sub Main()
        m_timer = New System.Timers.Timer(300000)
        m_timer.Enabled = True
        Dim clsList As New listJobsMultipleStates
        clsList.Start()
        Console.ReadLine()
    End Sub
    Private Sub m_timer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles m_timer.Elapsed
        Dim clsList As New listJobsMultipleStates
        clsList.Start()
    End Sub
End Module
