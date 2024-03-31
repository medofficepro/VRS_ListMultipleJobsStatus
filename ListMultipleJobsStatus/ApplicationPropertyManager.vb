Imports System.Configuration
Imports AnyModalCds


''' <summary>
'''     A lot of the settings for this simulation are "hard-coded" and configurable in the app.config file.
'''     Customer implementations would obviously not use the same author ID for all jobs, so author information would need to be persisted
'''     in the "upload queue" with the audio to be uploaded.
''' </summary>
Public Class ApplicationPropertyManager
#Region "CDS Properties"

    Private Shared _authorObject As ObjectId

    Public Shared ReadOnly Property AuthorObject() As ObjectId
        Get

            If _authorObject Is Nothing Then
                _authorObject = ObjectIdTools.construct(Author)
            End If

            Return _authorObject
        End Get
    End Property

#End Region

#Region "AppSettings"

    Private Shared _Author As String

    Private Shared _AudioFilePath As String

    Private Shared _OutputPath As String

    Private Shared _TestSplitJobs As System.Nullable(Of Boolean)

    Private Shared _SplitJobDraftPath As String

    Public Shared ReadOnly Property Author() As String
        Get
            Return SetLocalIfNull("Author", _Author)
        End Get
    End Property

    Public Shared ReadOnly Property AudioFilePath() As String
        Get
            Return SetLocalIfNull("AudioFilePath", _AudioFilePath)
        End Get
    End Property

    Public Shared ReadOnly Property OutputPath() As String
        Get
            Return SetLocalIfNull("OutputPath", _OutputPath)
        End Get
    End Property

    Public Shared ReadOnly Property TestSplitJobs() As Boolean
        Get
            Return SetLocalIfNull("TestSplitJobs", _TestSplitJobs)
        End Get
    End Property

    Public Shared ReadOnly Property SplitJobDraftPath() As String
        Get
            Return SetLocalIfNull("SplitJobDraftPath", _SplitJobDraftPath)
        End Get
    End Property
    Public Shared ReadOnly Property ConnectionString() As String
        Get
            Return SetLocalIfNull("ConnectionString", _ConnectionString)
        End Get
    End Property
    Private Shared _ConnectionString As String
    Public Shared ReadOnly Property MTLevel() As String
        Get
            Return SetLocalIfNull("MTLevel", _MTLevel)
        End Get
    End Property
    Private Shared _MTLevel As String

    Public Shared ReadOnly Property QALevel() As String
        Get
            Return SetLocalIfNull("QALevel", _QALevel)
        End Get
    End Property
    Private Shared _QALevel As String
    Public Shared ReadOnly Property VRSLevel() As String
        Get
            Return SetLocalIfNull("VRSLevel", _VRSLevel)
        End Get
    End Property
    Private Shared _VRSLevel As String
#End Region

    ''' <summary>
    '''     If propertyLocal is empty, set it to AppSetting specified at key field
    '''     return the specified propertyLocal
    ''' </summary>
    ''' <param name="key">key into app settings</param>
    ''' <param name="propertyLocal">local backing field for property, passed by reference</param>
    ''' <returns>value of (possibly updated) propertyLocal</returns>
    Private Shared Function SetLocalIfNull(ByVal key As String, ByRef propertyLocal As String) As String
        If [String].IsNullOrEmpty(propertyLocal) Then
            propertyLocal = ConfigurationManager.AppSettings(key)
        End If
        Return propertyLocal
    End Function

    Private Shared Function SetLocalIfNull(ByVal key As String, ByRef propertyLocal As System.Nullable(Of Boolean)) As Boolean
        If propertyLocal Is Nothing Then
            propertyLocal = Boolean.Parse(ConfigurationManager.AppSettings(key))
        End If
        Return CBool(propertyLocal)
    End Function
End Class

