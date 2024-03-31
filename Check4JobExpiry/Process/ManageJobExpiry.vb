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
    Public Class JobExpiry
        ''' <summary>
        ''' Find jobs on the queue that are out of TAT
        ''' </summary>
        Public Sub Start()
            Dim aTask As New DataAccess
            Dim DTList As New DataTable
            Dim DR As DataRow
            Dim TransID As String = String.Empty
            Try
                DTList = aTask.getExpiredJobsList
                For Each DR In DTList.Rows
                    If Not getStatus(ObjectIdTools.construct(DR("Job_ObjID").ToString)) Then
                        Dim clsDA As New DataAccess
                        With clsDA
                            .TranscriptionID = DR("TranscriptionID").ToString
                            TransID = DR("TranscriptionID").ToString
                            'routing for transcription from the scratch
                            .Status = DBJobstatus.MonitorJobFailed
                            .ErrorDesc = "JobExpiry: improper CDS state"
                            .UpdateTask()
                        End With
                        clsDA = Nothing
                    End If
                Next


                DTList = aTask.getCheckedOutList
                For Each DR In DTList.Rows

                    Dim clsDA As New DataAccess
                    With clsDA
                        .TranscriptionID = DR("TranscriptionID").ToString
                        'routing for transcription from the scratch
                        .Status = 1
                        .ErrorDesc = "CheckedOut Status: Took more than expected TAT"
                        .UpdateTranscriptionStatus()
                    End With
                    clsDA = Nothing

                Next

            Catch ex As Exception
                Dim SW As IO.StreamWriter = Nothing
                SW = New IO.StreamWriter("D:\Check4Expiry.txt", True)
                SW.WriteLine(TransID & " :" & ex.Message)
                SW.Close()
            Finally
                DTList.Dispose()
                aTask = Nothing
            End Try
        End Sub


        ''' <summary>
        ''' get job status and rout to the transcription queue in case not in appropriate state else update Jobstate
        ''' </summary>
        Private Function getStatus(ByVal JobID As ObjectId) As Boolean
            Dim lstJS As New WSJobControlImplService
            lstJS = CdsServiceFactory.GetJobControlService
            Dim JSs As JobStatus = Nothing
            Try
                JSs = lstJS.getStatus(JobID)
            Catch ex As Exception
                Console.WriteLine("{0} -> {1}", ObjectIdTools.ToString(JobID), ex.Message)
                Return False
            Finally
                lstJS.Dispose()
            End Try
            Try
                If Not JSs Is Nothing Then
                    Dim SW As IO.StreamWriter = Nothing
                    SW = New IO.StreamWriter("D:\MModalLogs\getStatus_" & JSs.externalId & ".txt", False)
                    SW.WriteLine(JSs.state)
                    SW.Close()
                    Console.WriteLine("{0} [{2}] -> {1}", JSs.externalId, ObjectIdTools.ToString(JSs.jobId), JSs.state)
                    Dim StatusID As Integer
                    Select Case JSs.state
                        Case 0
                            StatusID = 1
                        Case 1
                            StatusID = 2
                        Case 2
                            StatusID = 3
                        Case 3
                            StatusID = 4
                    End Select
                    Dim clsDA As New DataAccess
                    With clsDA
                        .TranscriptionID = JSs.externalId
                        .JobState = JSs.state
                        If JSs.state = AnyModalCds.JobState.CANCELLED Or JSs.state = AnyModalCds.JobState.FAILED Or JSs.state = AnyModalCds.JobState.UNDEFINED Then
                            'Still in unexpected JobState, routing for transcription from the scratch
                            .Status = DBJobstatus.MonitorJobFailed
                        ElseIf StatusID <> 0 Then
                            .Status = StatusID
                        End If
                        Return .UpdateTask()
                    End With
                    clsDA = Nothing
                End If
            Catch ex As Exception
                Console.WriteLine("{0} [{2}] -> {1}", JSs.externalId, ObjectIdTools.ToString(JSs.jobId), ex.Message)
                Return False
            End Try
        End Function
    End Class
End Namespace
