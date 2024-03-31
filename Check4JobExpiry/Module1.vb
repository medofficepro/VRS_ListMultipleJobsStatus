Imports Check4JobExpiry.Process
Imports System.Threading
Module Module1
    Private WithEvents m_timer As System.Timers.Timer
    Sub Main()
        m_timer = New System.Timers.Timer(600000)
        m_timer.Enabled = True
        Dim clsEx As New JobExpiry
        clsEx.Start()
        Console.ReadLine()
    End Sub
    Private Sub m_timer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles m_timer.Elapsed
        Dim clsEx As New JobExpiry
        clsEx.Start()
    End Sub
End Module
