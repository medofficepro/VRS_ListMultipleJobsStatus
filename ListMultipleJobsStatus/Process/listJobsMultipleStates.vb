Imports System.Threading
Imports AnyModalCds
Imports MModal.CLW.Factory
Imports MModal.CLW.Tasks
Imports System.Data.SqlClient
Imports SXFDBStatus
Imports SXFDBStatus.Constants
Imports SXFDBStatus.DataAccess
Imports SXFDBStatus.ApplicationPropertyManager
Namespace Process
    Public Class listJobsMultipleStates
        ''' <summary>
        ''' Find jobs on the queue that are ready to monitor and process them
        ''' </summary>
        Private ErrLog As New ErrorLogger
        Public Sub Start()
            ErrLog.WriteToEventLog("Started")
            ListJobs()
            listStuckJobs()

            'Dim aTask As New DataAccess
            'Dim DTList As DataTable = aTask.getMultipleJobsStateList
            'Dim AuthorIDS() As ObjectId
            'Try
            '    Dim i As Integer = 0
            '    ReDim AuthorIDS(DTList.Rows.Count - 1)
            '    For Each DR As DataRow In DTList.Rows
            '        AuthorIDS(i) = ObjectIdTools.construct(DR("Aut_ObjID"), "A~Author")
            '        i = i + 1
            '    Next
            '    If AuthorIDS.Length > 0 Then
            '        ListJobs(AuthorIDS)
            '    End If
            'Catch ex As Exception
            '    Console.WriteLine(" StartTime: " & Now & ": " & ex.Message)
            'Finally
            '    DTList.Dispose()
            '    aTask = Nothing
            'End Try
        End Sub
        Private Function listStuckJobs()
            Dim aTask As New DataAccess
            Dim DTList As DataTable = aTask.getStuckJobList

            Try
                Dim sqlQuery As String = String.Empty
                Console.WriteLine(" StuckJobs: " & DTList.Rows.Count)
                For Each DR As DataRow In DTList.Rows

                    Dim TransID As String = DR("TranscriptionID").ToString
                    Console.WriteLine(" TransID: " & TransID)
                    sqlQuery = String.Empty
                    If IO.File.Exists("d:\MModalLogs\" & TransID & ".txt") Then
                        Dim SW As IO.StreamReader = Nothing
                        SW = New IO.StreamReader("d:\MModalLogs\" & TransID & ".txt")
                        sqlQuery = SW.ReadToEnd()
                        SW.Close()

                    End If
                    If Not String.IsNullOrEmpty(sqlQuery) Then
                        Console.WriteLine(sqlQuery)
                        If aTask.updateQuery(sqlQuery) Then
                            IO.File.Move("d:\MModalLogs\" & TransID & ".txt", "d:\MModalLogs\Done\" & TransID & ".txt")
                        End If
                    End If

                Next
                Dim oDirInfo As New IO.DirectoryInfo("d:\MModalLogs\Insert")

                For Each oFile As IO.FileInfo In oDirInfo.GetFiles
                    sqlQuery = String.Empty
                    Dim SW As IO.StreamReader = Nothing
                    SW = New IO.StreamReader(oFile.FullName)
                    sqlQuery = SW.ReadToEnd()
                    SW.Close()
                    If Not String.IsNullOrEmpty(sqlQuery) Then
                        Console.WriteLine(sqlQuery)
                        If aTask.updateQuery(sqlQuery) Then
                            IO.File.Move(oFile.FullName, "d:\MModalLogs\Done\" & oFile.Name)
                        End If
                    End If
                Next
                
            Catch ex As Exception
                Console.WriteLine(" StartTime: " & Now & ": " & ex.Message)
            Finally
                DTList.Dispose()
                aTask = Nothing
            End Try
        End Function
        ''' <summary>
        ''' Just update JobStates to the Database
        ''' </summary>
        Private Function ListJobs(Optional ByVal AuthorIDS() As ObjectId = Nothing) As Boolean
            Dim EndTime As DateTime = DateAdd(DateInterval.Second, 30, Now)
            Dim StartTime As DateTime = DateAdd(DateInterval.Second, -30, EndTime.Subtract(TimeSpan.FromMinutes(5)))
            'Console.WriteLine("Now: " & Now & " StartTime: " & StartTime & " -  EndTime: " & EndTime)
            Dim TI As TimeInterval = TimeTools.constructInterval(StartTime, EndTime)
            Dim States() As Integer = New Integer(3) {AnyModalCds.JobState.UNDEFINED, AnyModalCds.JobState.PROCESSED, AnyModalCds.JobState.DEFINED, AnyModalCds.JobState.SCHEDULED}
            Dim lstJS As New WSJobControlImplService
            lstJS = CdsServiceFactory.GetJobControlService
            Dim JSs() As JobStatus = Nothing
            Try
                JSs = lstJS.listJobsMultipleStates(TI, States, AuthorIDS, 10000)
            Catch ex As Exception
                Return False
            Finally
                lstJS.Dispose()
            End Try

            If JSs Is Nothing OrElse 0 = JSs.Length Then
                Console.WriteLine(" StartTime: " & StartTime & ": No result")
            Else
                Console.WriteLine("{0} job(s)", JSs.Length)
                Dim clsDA As New DataAccess
                For Each js As JobStatus In JSs
                    ErrLog.WriteToEventLog("{0} [{2}] -> {1}", js.externalId, ObjectIdTools.ToString(js.jobId), js.state)
                    Console.WriteLine("{0} [{2}] -> {1}", js.externalId, ObjectIdTools.ToString(js.jobId), js.state)
                    Try
                        'Just update the JobState
                        clsDA = New DataAccess
                        With clsDA
                            .TranscriptionID = js.externalId
                            .JobState = js.state
                            If Not js.jobId Is Nothing Then .Job_ObjID = ObjectIdTools.ToString(js.jobId)
                            If Not js.documentId Is Nothing Then .Doc_ObjID = ObjectIdTools.ToString(js.documentId)
                            .QualityScore = js.expectedQuality
                            .UpdateTask()
                        End With
                    Catch ex As Exception
                        'do nothing
                        ErrLog.WriteToEventLog("{0} [{2}] -> {1}", js.externalId, ObjectIdTools.ToString(js.jobId), "error occured while updating status: " & ex.Message)
                        Console.WriteLine("{0} [{2}] -> {1}", js.externalId, ObjectIdTools.ToString(js.jobId), "error occured while updating status: " & ex.Message)
                    Finally
                        clsDA = Nothing
                    End Try
                Next
            End If
        End Function
    End Class
End Namespace
