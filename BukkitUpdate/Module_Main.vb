Imports System.IO
Imports System.Net
Imports System.Text

Module Module_Main
#Region "Main"
    Dim CurPath As String = My.Application.Info.DirectoryPath

    Sub Main()
        MySettings.InitSettings()
        Select Case My.Application.CommandLineArgs.Count

            Case 1
                Dim arg As String = My.Application.CommandLineArgs.Item(0).ToString
                Select Case arg.ToLower
                    Case "settings" : Settings()
                    Case "getcurcbver" : Console.WriteLine(getCurCBVer(MySettings.ServerDir & "\craftbukkit.jar"))
                    Case "help" : PrintHelp()
                    Case "?" : PrintHelp()
                    Case Else : Console.WriteLine("wrong usage, please read the help")
                End Select


            Case 2
                Dim BuildInfo As String = Nothing
                Dim arg As String = My.Application.CommandLineArgs.Item(0).ToString
                Dim arg2 As String = My.Application.CommandLineArgs.Item(1).ToString

                Select Case arg.ToLower
                    Case "getcurcbver"
                        Console.WriteLine(getCurCBVer(My.Application.CommandLineArgs.Item(1)))
                    Case "bukkitrecommended"
                        BuildInfo = Jenkins.DevPages.BukkitRecommended
                    Case "bukkitstable"
                        BuildInfo = Jenkins.DevPages.BukkitStable
                    Case "craftbukkitrecommended"
                        BuildInfo = Jenkins.DevPages.CraftBukkitRecommended
                    Case "craftbukkitstable"
                        BuildInfo = Jenkins.DevPages.CraftBukkitStable
                    Case "bukkitcurrent"
                        BuildInfo = "http://ci.bukkit.org/job/dev-Bukkit/" & MySettings.BukkitVersion
                    Case "craftbukkitcurrent"
                        BuildInfo = "http://ci.bukkit.org/job/dev-CraftBukkit/" & MySettings.CraftBukkitVersion
                    Case "thereisacow"
                        thereisacow(arg2)
                    Case Else
                        Console.WriteLine("wrong usage, please read the help")
                End Select
                If BuildInfo = Nothing = False Then
                    Jenkins.APIInfo.GetAPIInfo(BuildInfo & "/api/xml")
                    Select Case arg2.ToLower
                        Case "timestamp" : Console.WriteLine(Jenkins.APIInfo.timestamp)
                        Case "author" : Console.WriteLine(Jenkins.APIInfo.author)
                        Case "builddate" : Console.WriteLine(Jenkins.APIInfo.buildDate)
                        Case "buildduration" : Console.WriteLine(Jenkins.APIInfo.buildDuration)
                        Case "buildnumber" : Console.WriteLine(Jenkins.APIInfo.buildNumber)
                        Case "comment" : Console.WriteLine(Jenkins.APIInfo.comment)
                        Case "description" : Console.WriteLine(Jenkins.APIInfo.description)
                        Case "filename" : Console.WriteLine(Jenkins.APIInfo.fileName)
                        Case "displayname" : Console.WriteLine(Jenkins.APIInfo.fullDisplayName)
                        Case "url" : Console.WriteLine(Jenkins.APIInfo.fullUrl)
                        Case "download" : DownloadArtifact(, CurPath)
                        Case "all" : Jenkins.APIInfo.PrintAPIInfo()
                        Case "update" : Updater()
                        Case Else : Console.WriteLine("couldn't find this kind of argument")
                    End Select
                End If

            Case 3
                Dim arg As String = My.Application.CommandLineArgs.Item(0).ToString
                Select Case arg.ToLower
                    Case "settings" : MySettings.XMLChangeValue(My.Application.CommandLineArgs.Item(1), My.Application.CommandLineArgs.Item(2))
                    Case "craftbukkitstable"
                        Select Case My.Application.CommandLineArgs.Item(1).ToLower
                            Case "update"
                                Jenkins.APIInfo.GetAPIInfo(Jenkins.DevPages.CraftBukkitStable & "/api/xml")
                                Updater(My.Application.CommandLineArgs.Item(2))
                            Case "download"
                                Jenkins.APIInfo.GetAPIInfo(Jenkins.DevPages.CraftBukkitStable & "/api/xml")
                                DownloadArtifact(, My.Application.CommandLineArgs.Item(2))
                        End Select

                    Case "craftbukkitrecommended"
                        Select Case My.Application.CommandLineArgs.Item(1).ToLower
                            Case "update"
                                Jenkins.APIInfo.GetAPIInfo(Jenkins.DevPages.CraftBukkitRecommended & "/api/xml")
                                Updater(My.Application.CommandLineArgs.Item(2))
                            Case "download"
                                Jenkins.APIInfo.GetAPIInfo(Jenkins.DevPages.CraftBukkitRecommended & "/api/xml")
                                DownloadArtifact(, My.Application.CommandLineArgs.Item(2))
                        End Select

                    Case "bukkitstablerecommended"
                        Select Case My.Application.CommandLineArgs.Item(1).ToLower
                            Case "update"
                                Jenkins.APIInfo.GetAPIInfo(Jenkins.DevPages.BukkitRecommended & "/api/xml")
                                Updater(My.Application.CommandLineArgs.Item(2))
                            Case "download"
                                Jenkins.APIInfo.GetAPIInfo(Jenkins.DevPages.BukkitRecommended & "/api/xml")
                                DownloadArtifact(, My.Application.CommandLineArgs.Item(2))
                        End Select

                    Case "bukkitstable"
                        Select Case My.Application.CommandLineArgs.Item(1).ToLower
                            Case "update"
                                Jenkins.APIInfo.GetAPIInfo(Jenkins.DevPages.BukkitStable & "/api/xml")
                                Updater(My.Application.CommandLineArgs.Item(2))
                            Case "download"
                                Jenkins.APIInfo.GetAPIInfo(Jenkins.DevPages.BukkitStable & "/api/xml")
                                DownloadArtifact(, My.Application.CommandLineArgs.Item(2))
                        End Select

                    Case Else
                        Console.WriteLine("wrong usage, please read the help")
                End Select
            Case Else
                Console.WriteLine("wrong ammount of arguments")

        End Select
    End Sub
#End Region

#Region "Settings"
    Public Structure MySettings
        Shared ServerDir As String
        Shared BukkitVersion As Integer
        Shared CraftBukkitVersion As Integer
        Shared AlwaysBackup As Boolean

        Shared Sub InitSettings()
            If My.Computer.FileSystem.FileExists(CurDir() & "\settings.xml") Then
                Dim XMLReader As Xml.XmlReader = New Xml.XmlTextReader(CurPath & "\settings.xml")

                With XMLReader
                    Do While .Read
                        Select Case .NodeType
                            Case Xml.XmlNodeType.Element
                                Select Case .Name
                                    Case "BukkitVer" : BukkitVersion = .ReadString
                                    Case "CraftBukkitVer" : CraftBukkitVersion = .ReadString
                                    Case "ServerDir" : ServerDir = .ReadString
                                    Case "AlwaysBackup" : AlwaysBackup = .ReadString
                                End Select
                        End Select
                    Loop

                    .Close()
                End With
            Else


                Console.WriteLine("This seems to be your first start (no 'settings.xml' found) Please fill in some Settings" & vbCrLf)

                Console.WriteLine("Current Server Directory: ")
                Dim sd As String = Console.ReadLine()
                Dim readcbversion As String = getCurCBVer(sd & "\craftbukkit.jar")

                Console.WriteLine(vbCrLf & "Current Bukkit Version: ")
                Dim bv As Integer = Console.ReadLine()

                Console.WriteLine(vbCrLf & "If " & readcbversion & " isn't your current CraftBukkit Version, please enter yours, else leave blank")
                Dim cbv As String = Console.ReadLine()
                If cbv = "" Then cbv = readcbversion

                Console.WriteLine(vbCrLf & "Always Backup old Serverfiles: (true/false)")
                Dim ab As Boolean = Console.ReadLine()

                WriteNewXML(bv, cbv, sd, ab)

            End If
        End Sub


        Public Shared Sub XMLChangeValue(ByVal elementtochange As String, ByVal newvalue As String)
            Dim myXmlDocument As New Xml.XmlDocument
            myXmlDocument.Load(CurPath & "\settings.xml")

            Dim node As Xml.XmlNode
            node = myXmlDocument.DocumentElement

            Dim node2 As Xml.XmlNode

            For Each node In node.ChildNodes

                For Each node2 In node.ChildNodes

                    If node2.Name = elementtochange Then
                        node2.InnerText = newvalue
                        myXmlDocument.Save(CurPath & "\settings.xml")
                    End If

                Next
            Next
        End Sub

        Public Shared Sub WriteNewXML(ByVal BukkitVer As Integer, ByVal CraftBukkitVer As Integer, ByVal ServerDire As String, ByVal AlwaysBackupi As Boolean)

            Dim NewXML As New Xml.XmlTextWriter(CurDir() & "\settings.xml", Text.Encoding.UTF8)
            With NewXML
                .WriteStartDocument()
                .WriteComment("BukkitUpdate Configuration File")
                .WriteComment("If you don't know what you're changing let it be")
                .WriteStartElement("Settings")
                .WriteStartElement("Versions")
                .WriteElementString("BukkitVer", BukkitVer)
                .WriteElementString("CraftBukkitVer", CraftBukkitVer)
                .WriteEndElement()
                .WriteStartElement("Paths")

                .WriteElementString("ServerDir", ServerDire)
                .WriteElementString("AlwaysBackup", AlwaysBackupi)
                .WriteEndElement()
                .WriteEndDocument()
                .Flush()
                .Close()
            End With
        End Sub

        Public Shared Sub WriteSampleXML()

            Dim NewXML As New Xml.XmlTextWriter(CurDir() & "\settings.xml", Text.Encoding.UTF8)
            With NewXML
                .WriteStartDocument()
                .WriteComment("BukkitUpdate Configuration File")
                .WriteComment("If you don't know what you're changing let it be")
                .WriteStartElement("Settings")
                .WriteStartElement("Versions")
                .WriteElementString("BukkitVer", "000")
                .WriteElementString("CraftBukkitVer", "000")
                .WriteEndElement()
                .WriteStartElement("Paths")

                .WriteElementString("ServerDir", "C:\Minecraft\Bukkit")
                .WriteElementString("AlwaysBackup", True)
                .WriteEndElement()
                .WriteEndDocument()
                .Flush()
                .Close()
            End With
        End Sub

    End Structure
#End Region

#Region "Misc"


    Public Sub thereisacow(ByVal path As String)


    End Sub


    Public Sub Updater(Optional ByVal destin As String = Nothing)
        Dim arg1 As String = My.Application.CommandLineArgs.Item(0).ToLower

        Select Case arg1
            Case "bukkitstable", "bukkitrecommended"
                If Jenkins.APIInfo.buildNumber > MySettings.BukkitVersion Then
                    Console.WriteLine("There is an Update available for Bukkit: " & Jenkins.APIInfo.buildNumber & " (yours is " & MySettings.BukkitVersion & ")" & vbCrLf & "Do you want to download it now? (Y/N/I -for info)")
                    Select Case Console.ReadLine()
                        Case "Y", "y"
                            DownloadArtifact(, destin)
                        Case "I", "i"
                            Jenkins.APIInfo.PrintAPIInfo()
                    End Select
                Else
                    Console.WriteLine("There is no Update available for Bukkit")
                End If
            Case "craftbukkitstable", "craftbukkitrecommended"
                If Jenkins.APIInfo.buildNumber > MySettings.CraftBukkitVersion Then
                    Console.WriteLine("There is an Update available for Bukkit: " & Jenkins.APIInfo.buildNumber & " (yours is " & MySettings.CraftBukkitVersion & ")" & vbCrLf & "Do you want to download it now? (Y/N/I -for info)")
                    Select Case Console.ReadLine()
                        Case "Y", "y"
                            DownloadArtifact(, destin)
                        Case "I", "i"
                            Jenkins.APIInfo.PrintAPIInfo()
                    End Select
                Else
                    Console.WriteLine("There is no Update available for CraftBukkit")
                End If
            Case Else
                Console.WriteLine(arg1 & " is not Supported for Updating")
        End Select

    End Sub


    Public Sub PrintHelp()
        Dim an As String = My.Application.Info.AssemblyName & ".exe "
        Console.WriteLine(vbCrLf & "Commands:" & vbCrLf & vbCrLf & "<===========General-Commands===========>" & vbCrLf & _
                          an & "<settings> Adjusts Settings" & vbCrLf & _
                          an & "<getcurcbver> [pathtoJAR] Prints Current CraftBukkit Version" & vbCrLf & "if no [pathtoJar] is set, " & an & " will use the default serverdirectory" & vbCrLf & _
                          an & "<help> or <?> Prints this help" & vbCrLf & vbCrLf & "<===========Jenkins-Commands===========>" & vbCrLf & _
                          an & "<bukkit*> or <craftbukkit*> <argument>" & vbCrLf & _
                          "replace * with either recommended, stable or current" & vbCrLf & vbCrLf & _
                          "list of arguments: (type them without the " & Chr(34) & "- " & Chr(34) & ")" & vbCrLf & vbCrLf & _
                          vbTab & "- timestamp" & vbCrLf & _
                          vbTab & "- author" & vbCrLf & _
                          vbTab & "- builddate" & vbCrLf & _
                          vbTab & "- buildduration" & vbCrLf & _
                          vbTab & "- buildnumber" & vbCrLf & _
                          vbTab & "- comment" & vbCrLf & _
                          vbTab & "- description" & vbCrLf & _
                          vbTab & "- filename" & vbCrLf & _
                          vbTab & "- displayname" & vbCrLf & _
                          vbTab & "- url" & vbCrLf & _
                          vbTab & "- all (Prints all Infos listed above)" & vbCrLf & _
                          vbTab & "- Update [DestinationPath] (Just test it)" & vbCrLf & _
                          vbTab & "- download [DestinationPath]" & vbCrLf & "(if no DestinationPath is set " & an & " will download it to the server dir")
    End Sub

    Private Sub Settings()
        Console.WriteLine("1. Set Craftbukkit Server Directory")
        Console.WriteLine("2. Set Bukkit version")
        Console.WriteLine("3. Manually set CraftBukkit version (not recommended)")
        Console.WriteLine("4. Set AlwaysBackup")
        Select Case Console.ReadLine
            Case 1
                Console.WriteLine(vbCrLf & "Full Path to the Server Directory:" & vbCrLf)
                MySettings.XMLChangeValue("ServerDir", Console.ReadLine())
            Case 2
                Console.WriteLine(vbCrLf & "New bukkit version: (Format: ###)" & vbCrLf)
                MySettings.XMLChangeValue("BukkitVer", Console.ReadLine)
            Case 3
                Console.WriteLine(vbCrLf & "New craftbukkit version: (Format: ###)" & vbCrLf)
                MySettings.XMLChangeValue("CraftBukkitVer", Console.ReadLine)
            Case 4
                Console.WriteLine(vbCrLf & "Always Backup old Serverfiles: (If set to true " & My.Application.Info.AssemblyName & ".exe will backup old serverfiles)" & vbCrLf)
                MySettings.XMLChangeValue("AlwaysBackup", Console.ReadLine)
        End Select
    End Sub

    Public Function getCurCBVer(ByVal CraftBukkitPath As String)

        Try

            Dim JRE As New Process
            With JRE.StartInfo
                If My.Computer.FileSystem.FileExists("C:\Program Files (x86)\Java\jre6\bin\javaw.exe") Then
                    .FileName = "C:\Program Files (x86)\Java\jre6\bin\javaw.exe"
                ElseIf My.Computer.FileSystem.FileExists("C:\Program Files\Java\jre6\bin\javaw.exe") Then
                    .FileName = "C:\Program Files\Java\jre6\bin\javaw.exe"
                ElseIf My.Computer.FileSystem.FileExists("C:\Windows\system32\javaw.exe") Then
                    .FileName = "C:\Windows\system32\javaw.exe"
                Else
                    Console.WriteLine("Cant get the path to your java.exe")
                    Console.Write("please enter it here: ")
                    .FileName = Console.ReadLine()
                End If

                .Arguments = " -jar " & CraftBukkitPath & " -v"
                .UseShellExecute = False
                .RedirectStandardOutput = True
            End With
            JRE.Start()
            Dim baseStr As String = JRE.StandardOutput.ReadLine
            Dim basLft As Integer = baseStr.IndexOf("-b") + 2
            Dim baseRgt As Integer = baseStr.LastIndexOf("jnks")
            Return baseStr.Substring(basLft, baseRgt - basLft)

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

#End Region

#Region "Downloader"

    Dim WithEvents wc As New Net.WebClient
    Dim WithEvents tim As New Timers.Timer

    Dim zm As Integer = 0
    Dim dl_fin As Boolean = False
    Dim output As String
    Dim ebrec, ebtorec, eprog As Integer

    Private Sub tim_change() Handles tim.Elapsed

        Console.Write(Math.Round((ebrec - zm) / 1024 * 2) & " KB/s " & Math.Round(ebrec / 1048576, 2) & "MB/" & Math.Round(ebtorec / 1048576, 2) & "MB [" & eprog & "%]   ")
        zm = ebrec
        Console.CursorLeft = 0
    End Sub


    Private Sub wc_DownloadProggressChanged(ByVal sender As Object, ByVal e As Net.DownloadProgressChangedEventArgs) Handles wc.DownloadProgressChanged
        ebrec = e.BytesReceived
        ebtorec = e.TotalBytesToReceive
        eprog = e.ProgressPercentage
    End Sub

    Private Sub wc_DownloadFileCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs) Handles wc.DownloadFileCompleted
        tim.Stop()
        If updatebool = True Then
            If artifact_short = "bukkit.jar" Then
                MySettings.XMLChangeValue("BukkitVer", Jenkins.APIInfo.buildNumber)
            Else
                MySettings.XMLChangeValue("CraftBukkitVer", Jenkins.APIInfo.buildNumber)
            End If
        Else
            Console.WriteLine("Succesfully downloaded " & New IO.FileInfo(destination_final).Name & " to " & destination_final)
        End If

        Console.WriteLine("Download Finished; Press any Key to Continue   ")
        dl_fin = True
        Console.CursorVisible = True
    End Sub

    Dim destination_final As String
    Dim artifact_short As String
    Dim updatebool As Boolean = True

    Public Sub DownloadArtifact(Optional ByVal BuildAPI As String = Nothing, Optional ByVal DestinationDir As String = Nothing)

        If BuildAPI = Nothing = False Then
            Jenkins.APIInfo.GetAPIInfo(BuildAPI)
        End If

        Dim source As New Uri(Jenkins.APIInfo.fullUrl & "artifact/" & Jenkins.APIInfo.relativePath)

        If Jenkins.APIInfo.fileName.StartsWith("bukkit") Then
            artifact_short = "bukkit.jar"
        Else
            artifact_short = "craftbukkit.jar"
        End If

        If DestinationDir = Nothing Then
            destination_final = MySettings.ServerDir & "\" & artifact_short
        Else
            updatebool = False
            destination_final = DestinationDir & "\" & artifact_short
        End If

        If My.Computer.FileSystem.FileExists(destination_final) Then
            If MySettings.AlwaysBackup Then
                My.Computer.FileSystem.RenameFile(destination_final, artifact_short & ".bak_" & Today.Day & Today.Month & Today.Year & Now.Hour & Now.Minute & Now.Millisecond)
            Else
                Console.WriteLine("Do you want to Backup your old " & artifact_short & "? (Y/N)")
                Select Case Console.ReadLine
                    Case "Y", "y"
                        My.Computer.FileSystem.RenameFile(destination_final, artifact_short & ".bak_" & Today.Day & Today.Month & Today.Year & Now.Hour & Now.Minute & Now.Millisecond)
                    Case "N", "n"
                        My.Computer.FileSystem.DeleteFile(destination_final)
                    Case Else
                        Exit Sub
                End Select
            End If
        End If
        Console.CursorVisible = False
        tim.Interval = 500
        tim.Start()
        wc.DownloadFileAsync(source, destination_final)

        While dl_fin = False
            Console.ReadKey(True)
        End While

    End Sub

#End Region

#Region "Jenkins"
    Public Structure Jenkins


        Public Structure DevPages
            Shared BukkitRecommended As String = "http://ci.bukkit.org/job/dev-Bukkit/promotion/latest/Recommended"
            Shared BukkitStable As String = "http://ci.bukkit.org/job/dev-Bukkit/lastSuccessfulBuild"
            Shared CraftBukkitRecommended As String = "http://ci.bukkit.org/job/dev-CraftBukkit/promotion/latest/Recommended"
            Shared CraftBukkitStable As String = "http://ci.bukkit.org/job/dev-CraftBukkit/lastSuccessfulBuild"
        End Structure

        Public Shared Function getJobBuild(ByVal Build As String) As String
            Dim webclient As New Net.WebClient
            Return webclient.DownloadString(Build & "/buildNumber")
        End Function

        Public Structure APIInfo
            Shared fileName As String
            Shared description As String
            Shared buildDuration As String
            Shared fullDisplayName As String
            Shared buildNumber As String
            Shared timestamp As String
            Shared fullUrl As String
            Shared author As String
            Shared comment As String
            Shared buildDate As String
            Shared relativePath As String

            Public Shared Sub GetAPIInfo(ByVal Build As String)
                Dim XMLReader As Xml.XmlReader = New Xml.XmlTextReader(Build.ToString)

                With XMLReader
                    Do While .Read
                        Select Case .NodeType
                            Case Xml.XmlNodeType.Element
                                Dim tmp As String
                                Select Case .Name
                                    Case "fileName" : fileName = .ReadString
                                    Case "relativePath" : relativePath = .ReadString
                                    Case "description" : tmp = .ReadString : description = tmp.Replace("<br />", "")
                                    Case "duration" : buildDuration = .ReadString
                                    Case "fullDisplayName" : fullDisplayName = .ReadString
                                    Case "number" : buildNumber = .ReadString
                                    Case "timestamp" : timestamp = .ReadString
                                    Case "url" : tmp = .ReadString : fullUrl = tmp.Replace("ci-b.", "ci.")
                                    Case "fullName" : author = .ReadString
                                    Case "comment" : tmp = .ReadString : comment = tmp.Remove(tmp.Length - 1)
                                    Case "date" : buildDate = .ReadString
                                End Select
                        End Select
                    Loop
                    .Close()
                End With
            End Sub

            Public Shared Sub PrintAPIInfo()
                Console.WriteLine("Author: " & vbTab & Jenkins.APIInfo.author)
                Console.WriteLine("BuildDate: " & vbTab & Jenkins.APIInfo.buildDate)
                Console.WriteLine("BuildDuration: " & vbTab & Jenkins.APIInfo.buildDuration)
                Console.WriteLine("BuildNumber: " & vbTab & Jenkins.APIInfo.buildNumber)
                Console.WriteLine("Comment: " & vbTab & Jenkins.APIInfo.comment)
                Console.WriteLine("Description: " & vbTab & Jenkins.APIInfo.description)
                Console.WriteLine("Filename: " & vbTab & Jenkins.APIInfo.fileName)
                Console.WriteLine("DisplayName: " & vbTab & Jenkins.APIInfo.fullDisplayName)
                Console.WriteLine("FullURL: " & vbTab & Jenkins.APIInfo.fullUrl)
                Console.WriteLine("RelativePath: " & vbTab & Jenkins.APIInfo.relativePath)
                Console.WriteLine("Timestamp: " & vbTab & Jenkins.APIInfo.timestamp)
            End Sub
        End Structure

    End Structure
#End Region

End Module
