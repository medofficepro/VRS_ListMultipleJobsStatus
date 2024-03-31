Imports System.Diagnostics

<CLSCompliant(True)> _
Public Class ErrorLogger
    Public Sub New()
        'default constructor
    End Sub
    Public Function WriteToEventLog(ByVal entry As String, _
                    Optional ByVal appName As String = "VRS JobStatus", _
                    Optional ByVal eventType As  _
                    EventLogEntryType = EventLogEntryType.Information, _
                    Optional ByVal logName As String = "VRSJobStatus") As Boolean

        Dim objEventLog As New EventLog
        ' Dim SW As IO.StreamWriter = Nothing
        'SW = New IO.StreamWriter(IO.Path.Combine("c:\VRSLog", "log.txt"), True)
        Try


            'Register the Application as an Event Source
            If Not EventLog.SourceExists(appName) Then
                EventLog.CreateEventSource(appName, logName)
            End If
            'SW.WriteLine(entry & ":" & eventType.ToString)
            'log the entry
            objEventLog.Source = appName
            objEventLog.WriteEntry(entry, eventType)
            'MailNotification(eventType.ToString & ": " & entry)
            Return True

        Catch Ex As Exception
            'SW.WriteLine(entry & ":" & eventType.ToString & " Ex:" & Ex.Message)
            'MailNotification(eventType & ": " & entry & " Exception: " & Ex.Message)
            Return False
        Finally
            'SW.Dispose()
        End Try

    End Function
#Region "MAilAlert"
    Public Function MailNotification(ByVal strBody As String) As Boolean
        Dim clsMTC As New MTCService.MTClientService
        With clsMTC
            .SendMailAlerts("VRSService@edictate.com", "rgudadhe@edictate.com", "VRSService Alert", strBody)
        End With
        clsMTC = Nothing
    End Function
#End Region

End Class
