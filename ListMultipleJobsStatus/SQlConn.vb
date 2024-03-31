Imports System
Imports System.Data
Imports System.Data.SqlClient

Public Class SQlConn
    Private oConString As String
    Public Function Connect() As SqlConnection
        Try
            Dim oConn As New SQlConnection
            oConn.ConnectionString = oConString
            oConn.Open()
            If oConn.State = ConnectionState.Open Then
                Return oConn
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Function Close(ByVal oConn As SqlConnection) As Boolean
        Try
            If oConn.State <> ConnectionState.Closed Then
                oConn.Close()
            End If
            Return IIf(oConn.State = ConnectionState.Closed, True, False)
        Catch ex As Exception
            Return False
        Finally
            If Not oConn Is Nothing Then
                oConn.Dispose()
            End If
        End Try
    End Function

    Public Sub New()
        oConString = ApplicationPropertyManager.ConnectionString
    End Sub
End Class
