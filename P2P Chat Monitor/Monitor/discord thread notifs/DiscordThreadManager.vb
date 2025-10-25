Imports System.IO
Imports MaterialSkin
Imports MaterialSkin.Controls
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Partial Class DiscordThreadManager
    Inherits MaterialForm

    Private Class ThreadIdSet
        Public Property ChatsId As String
        Public Property TasksId As String
        Public Property QuestsId As String
        Public Property ErrorsId As String
        Public Property SelfiesId As String
    End Class

    Private Class RoutePersist
        Public Property Account As String
        Public Property ForumWebhook As String
        Public Property ShowChats As Boolean
        Public Property ShowTasks As Boolean
        Public Property ShowQuests As Boolean
        Public Property ShowErrors As Boolean
        Public Property ShowSelfies As Boolean
        Public Property ChatsId As String
        Public Property TasksId As String
        Public Property QuestsId As String
        Public Property ErrorsId As String
        Public Property SelfiesId As String
    End Class

    Private Class DTMState
        Public Property Options As Object
        Public Property Ids As ThreadIdSet
    End Class

    Private Const MAX_CARDS As Integer = 6
    Private Const TOP_MARGIN As Integer = 52
    Private Const LEFT_MARGIN As Integer = 4
    Private Const RIGHT_MARGIN As Integer = 9
    Private Const VERT_SPACING As Integer = 10
    Private _cardHeight As Integer = 82

    Private ReadOnly cards As New List(Of MaterialCard)
    Private ReadOnly selectedAccounts As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
    Private allAccounts As List(Of String) = New List(Of String)

    Public Sub New()
        InitializeComponent()

        Dim mgr = MaterialSkinManager.Instance
        mgr.EnforceBackcolorOnAllComponents = True
        Try
            mgr.AddFormToManage(Me)
        Catch
        End Try

        AddHandler Me.Shown, Sub(sender As Object, e As EventArgs)
                                 Try
                                     ReloadAccountsFromSettings()
                                     LoadRoutesFromCfg()
                                 Catch
                                 End Try
                             End Sub

        AddHandler Me.FormClosing, Sub(sender As Object, e As FormClosingEventArgs)
                                       Try
                                           SaveRoutesToCfg()
                                       Catch
                                       End Try
                                   End Sub

        Dim enableDark As Boolean = My.Settings.DarkModeOn
        If enableDark Then
            mgr.Theme = MaterialSkinManager.Themes.DARK
            mgr.ColorScheme = New ColorScheme(Primary.Grey800, Primary.Grey900, Primary.Grey500, Accent.LightBlue200, TextShade.WHITE)
        Else
            mgr.Theme = MaterialSkinManager.Themes.LIGHT
            mgr.ColorScheme = New ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE)
        End If
        canvas.BackColor = Me.BackColor

        _cardHeight = managedAccount1.Height
        AnchorCard(managedAccount1)
        HookCard(managedAccount1)
        cards.Add(managedAccount1)
        ReloadAccountsFromSettings()
        RefreshAllAccountCombos()
        AddHandler add1.Click, AddressOf AddManagedAccount_Click
        AddHandler canvas.Resize, Sub() LayoutCards()
        AddHandler Me.Resize, Sub() LayoutCards()
        LayoutCards()
    End Sub

    Public ReadOnly Property CanvasPanel As Panel
        Get
            Return canvas
        End Get
    End Property

    Private Sub ReloadAccountsFromSettings()
        allAccounts = New List(Of String)()

        Dim sources As List(Of String) = Nothing
        Try
            sources = main.GetLiveLogFolderPaths()
        Catch
            sources = Nothing
        End Try
        If sources Is Nothing Then sources = New List(Of String)()

        For Each p In sources
            Try
                Dim name As String = New DirectoryInfo(p).Name
                If Not allAccounts.Contains(name, StringComparer.OrdinalIgnoreCase) Then
                    allAccounts.Add(name)
                End If
            Catch
            End Try
        Next

        allAccounts = allAccounts.OrderBy(Function(s) s, StringComparer.OrdinalIgnoreCase).ToList()
    End Sub

    Private Sub RefreshAllAccountCombos(Optional except As MaterialComboBox = Nothing)
        For Each c In cards
            Dim cmb As MaterialComboBox = TryCast(FindChildByName(c, "accounts"), MaterialComboBox)
            If cmb Is Nothing OrElse cmb Is except Then Continue For
            RefreshOneAccountCombo(cmb)
        Next
    End Sub

    Private Sub RefreshOneAccountCombo(cmb As MaterialComboBox)
        Dim current As String = If(TryCast(cmb.SelectedItem, String), TryCast(cmb.Tag, String))
        Dim pool = New List(Of String)()

        For Each a In allAccounts
            If Not selectedAccounts.Contains(a) OrElse StringComparer.OrdinalIgnoreCase.Equals(a, current) Then
                pool.Add(a)
            End If
        Next

        pool.Sort(StringComparer.OrdinalIgnoreCase)

        cmb.BeginUpdate()
        Try
            cmb.Items.Clear()
            If pool.Count > 0 Then
                cmb.Items.AddRange(pool.Cast(Of Object).ToArray())
            End If

            If Not String.IsNullOrWhiteSpace(current) AndAlso pool.Contains(current, StringComparer.OrdinalIgnoreCase) Then
                Dim idx = pool.FindIndex(Function(s) StringComparer.OrdinalIgnoreCase.Equals(s, current))
                cmb.SelectedIndex = idx
            Else
                cmb.SelectedIndex = -1
            End If
        Finally
            cmb.EndUpdate()
        End Try
    End Sub

    Private Sub OnAccountSelectionCommitted(sender As Object, e As EventArgs)
        Dim cmb = TryCast(sender, MaterialComboBox)
        If cmb Is Nothing Then Return

        Dim oldSel As String = TryCast(cmb.Tag, String)
        If Not String.IsNullOrWhiteSpace(oldSel) Then selectedAccounts.Remove(oldSel)

        Dim newSel As String = TryCast(cmb.SelectedItem, String)
        If Not String.IsNullOrWhiteSpace(newSel) Then selectedAccounts.Add(newSel)

        cmb.Tag = newSel
        RefreshAllAccountCombos(except:=cmb)
    End Sub

    Private Sub AddManagedAccount_Click(sender As Object, e As EventArgs)
        If cards.Count >= MAX_CARDS Then
            MessageBox.Show(Me, "You can add up to 6 managed accounts on this page.", "Limit reached",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim newCard = CreateCardClone()
        canvas.Controls.Add(newCard)
        cards.Add(newCard)
        Dim cmb As MaterialComboBox = TryCast(FindChildByName(newCard, "accounts"), MaterialComboBox)
        If cmb IsNot Nothing Then RefreshOneAccountCombo(cmb)

        LayoutCards()
        newCard.BringToFront()
    End Sub

    Private Function CreateCardClone() As MaterialCard
        Dim card As New MaterialCard() With {
            .Depth = 0,
            .Padding = New Padding(14),
            .MouseState = MouseState.HOVER,
            .Size = New Size(Math.Max(100, canvas.ClientSize.Width - LEFT_MARGIN - RIGHT_MARGIN), _cardHeight)
        }

        Dim cmb As New MaterialComboBox() With {
            .AutoResize = False, .Depth = 0, .DrawMode = DrawMode.OwnerDrawVariable,
            .DropDownHeight = 174, .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Roboto Medium", 14.0F, FontStyle.Bold, GraphicsUnit.Pixel),
            .Hint = "Account", .IntegralHeight = False, .ItemHeight = 43,
            .Location = New Point(17, 17), .MaxDropDownItems = 4,
            .MouseState = MouseState.OUT, .Name = "accounts", .Size = New Size(146, 49)
        }
        AddHandler cmb.SelectionChangeCommitted, AddressOf OnAccountSelectionCommitted

        Dim tb As New MaterialTextBox() With {
            .AnimateReadOnly = False, .BorderStyle = BorderStyle.None, .Depth = 0,
            .Font = New Font("Roboto", 16.0F, FontStyle.Regular, GraphicsUnit.Pixel),
            .Hint = "Thread Webhook", .LeadingIcon = Nothing, .LeaveOnEnterKey = True,
            .Location = New Point(169, 17), .MaxLength = 200, .MouseState = MouseState.OUT,
            .Multiline = False, .Name = "txtWebhook", .Size = New Size(350, 50), .Text = ""
        }

        Dim btnMon As New MaterialButton() With {
            .AutoSizeMode = AutoSizeMode.GrowAndShrink, .Density = MaterialButton.MaterialButtonDensity.[Default],
            .Depth = 0, .HighEmphasis = True, .Icon = Nothing, .Location = New Point(526, 22),
            .Margin = New Padding(4, 6, 4, 6), .MouseState = MouseState.HOVER, .Name = "btnMonitorOptions",
            .NoAccentTextColor = Color.Empty, .Size = New Size(153, 36), .Text = "Monitor Options",
            .Type = MaterialButton.MaterialButtonType.Contained, .UseAccentColor = False
        }
        AddHandler btnMon.Click, Sub(_s, _e) OpenDTMOptions(card)

        Dim btnIDs As New MaterialButton() With {
            .AutoSizeMode = AutoSizeMode.GrowAndShrink, .Density = MaterialButton.MaterialButtonDensity.[Default],
            .Depth = 0, .HighEmphasis = True, .Icon = Nothing, .Location = New Point(687, 22),
            .Margin = New Padding(4, 6, 4, 6), .MouseState = MouseState.HOVER, .Name = "btnThreadIDs",
            .NoAccentTextColor = Color.Empty, .Size = New Size(107, 36), .Text = "Thread ID's",
            .Type = MaterialButton.MaterialButtonType.Contained, .UseAccentColor = False
        }
        AddHandler btnIDs.Click, Sub(_s, _e) OpenThreadIds(card)

        Dim fab As New MaterialFloatingActionButton() With {
            .AutoSizeMode = AutoSizeMode.GrowAndShrink, .Depth = 0, .Mini = True,
            .Icon = My.Resources.trash_can, .Location = New Point(801, 20),
            .MouseState = MouseState.HOVER, .Name = "deleteDTM", .Size = New Size(40, 40)
        }
        AddHandler fab.Click, Sub(_s, _e) RemoveCard(card)

        card.Controls.Add(cmb)
        card.Controls.Add(tb)
        card.Controls.Add(btnMon)
        card.Controls.Add(btnIDs)
        card.Controls.Add(fab)

        AnchorCard(card)
        card.BackColor = Me.BackColor
        card.ForeColor = Me.ForeColor
        Return card
    End Function

    Private Sub AnchorCard(c As MaterialCard)
        c.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
    End Sub

    Private Sub HookCard(c As MaterialCard)

        Dim fab = TryCast(FindChildByName(c, "deleteDTM"), MaterialFloatingActionButton)
        If fab IsNot Nothing Then AddHandler fab.Click, Sub(_s, _e) RemoveCard(c)

        Dim cmb = TryCast(FindChildByName(c, "accounts"), MaterialComboBox)
        If cmb IsNot Nothing Then
            AddHandler cmb.SelectionChangeCommitted, AddressOf OnAccountSelectionCommitted
        End If
        AnchorCard(c)

        Dim btnMon = TryCast(FindChildByName(c, "btnMonitorOptions"), MaterialButton)
        If btnMon IsNot Nothing Then
            AddHandler btnMon.Click, Sub(_s, _e) OpenDTMOptions(c)
        End If

        Dim btnIDs = TryCast(FindChildByName(c, "btnThreadIDs"), MaterialButton)
        If btnIDs IsNot Nothing Then
            AddHandler btnIDs.Click, Sub(_s, _e) OpenThreadIds(c)
        End If

    End Sub

    Private Sub RemoveCard(c As MaterialCard)
        Dim cmb = TryCast(FindChildByName(c, "accounts"), MaterialComboBox)
        If cmb IsNot Nothing Then
            Dim was As String = TryCast(cmb.Tag, String)
            If Not String.IsNullOrWhiteSpace(was) Then selectedAccounts.Remove(was)
        End If

        If cards.Contains(c) Then cards.Remove(c)
        canvas.Controls.Remove(c)
        c.Dispose()
        RefreshAllAccountCombos()
        LayoutCards()
    End Sub

    Private Sub LayoutCards()
        Dim y As Integer = TOP_MARGIN
        Dim usableW As Integer = Math.Max(100, canvas.ClientSize.Width - LEFT_MARGIN - RIGHT_MARGIN)

        For Each c In cards
            c.SuspendLayout()
            c.Location = New Point(LEFT_MARGIN, y)
            c.Width = usableW
            y += c.Height + VERT_SPACING
            c.ResumeLayout()
        Next
    End Sub

    Private Shared Function FindChildByName(parent As Control, name As String) As Control
        For Each ctl As Control In parent.Controls
            If String.Equals(ctl.Name, name, StringComparison.OrdinalIgnoreCase) Then Return ctl
        Next
        Return Nothing
    End Function
    Private Sub OpenDTMOptions(card As MaterialCard)
        Dim state As DTMState = TryCast(card.Tag, DTMState)
        If state Is Nothing Then
            state = New DTMState With {.Options = New DTMOpts(), .Ids = New ThreadIdSet()}
        ElseIf state.Options Is Nothing Then
            state.Options = New DTMOpts()
        End If

        Dim opts As DTMOpts = DirectCast(state.Options, DTMOpts)

        Using dlg As New DTMOptions(opts.Chats, opts.Tasks, opts.Quests, opts.Errors, opts.Selfies)
            If dlg.ShowDialog(Me) = DialogResult.OK Then
                opts.Chats = dlg.Chats
                opts.Tasks = dlg.Tasks
                opts.Quests = dlg.Quests
                opts.Errors = dlg.Errors
                opts.Selfies = dlg.Selfies
                state.Options = opts
                card.Tag = state
                Dim btn = TryCast(FindChildByName(card, "btnMonitorOptions"), MaterialButton)
                If btn IsNot Nothing Then btn.Text = "Monitor Options"
            End If
        End Using
    End Sub

    Private Sub OpenThreadIds(card As MaterialCard)
        Dim state As DTMState = TryCast(card.Tag, DTMState)
        If state Is Nothing Then
            state = New DTMState With {.Options = New DTMOpts(), .Ids = New ThreadIdSet()}
        ElseIf state.Options Is Nothing Then
            state.Options = New DTMOpts()
        End If
        If state.Ids Is Nothing Then state.Ids = New ThreadIdSet()

        Dim opts As DTMOpts = DirectCast(state.Options, DTMOpts)
        Dim ids As ThreadIdSet = state.Ids

        Using dlg As New ThreadIds(
            showChats:=opts.Chats,
            showTasks:=opts.Tasks,
            showQuests:=opts.Quests,
            showErrors:=opts.Errors,
            showSelfies:=opts.Selfies,
            initialChats:=ids.ChatsId,
            initialTasks:=ids.TasksId,
            initialQuests:=ids.QuestsId,
            initialErrors:=ids.ErrorsId,
            initialSelfies:=ids.SelfiesId
        )
            If dlg.ShowDialog(Me) = DialogResult.OK Then
                ids.ChatsId = dlg.ChatsId
                ids.TasksId = dlg.TasksId
                ids.QuestsId = dlg.QuestsId
                ids.ErrorsId = dlg.ErrorsId
                ids.SelfiesId = dlg.SelfiesId
                state.Ids = ids
                card.Tag = state
            End If
        End Using
    End Sub
    Private Shared Function CfgPath() As String
        Dim root = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        Dim dir = IO.Path.Combine(root, "DreamBot", "P2P Monitor")
        If Not IO.Directory.Exists(dir) Then IO.Directory.CreateDirectory(dir)
        Return IO.Path.Combine(dir, "settings.cfg")
    End Function

    Private Function FindWebhookTextBox(card As MaterialCard) As MaterialTextBox
        Dim tb = TryCast(FindChildByName(card, "txtWebhook"), MaterialTextBox)
        If tb Is Nothing Then tb = TryCast(FindChildByName(card, "MaterialTextBox1"), MaterialTextBox)
        Return tb
    End Function

    Private Function CardSelectedAccount(card As MaterialCard) As String
        Dim cmb = TryCast(FindChildByName(card, "accounts"), MaterialComboBox)
        If cmb Is Nothing Then Return ""
        Dim sel As String = Nothing
        If cmb.SelectedItem IsNot Nothing Then sel = cmb.SelectedItem.ToString()
        If String.IsNullOrWhiteSpace(sel) Then sel = TryCast(cmb.Tag, String)
        Return If(sel, "")
    End Function

    Private Function CollectRoutesFromUI() As List(Of RoutePersist)
        Dim list As New List(Of RoutePersist)()

        For Each card In cards
            Dim acct = CardSelectedAccount(card)
            If String.IsNullOrWhiteSpace(acct) Then Continue For

            Dim tb = FindWebhookTextBox(card)
            Dim webhook = If(tb IsNot Nothing, tb.Text.Trim(), "")

            Dim state As DTMState = TryCast(card.Tag, DTMState)
            If state Is Nothing Then state = New DTMState With {.Options = New DTMOpts(), .Ids = New ThreadIdSet()}
            If state.Options Is Nothing Then state.Options = New DTMOpts()
            If state.Ids Is Nothing Then state.Ids = New ThreadIdSet()

            Dim opts As DTMOpts = DirectCast(state.Options, DTMOpts)
            Dim ids As ThreadIdSet = state.Ids

            list.Add(New RoutePersist With {
            .Account = acct,
            .ForumWebhook = webhook,
            .ShowChats = opts.Chats,
            .ShowTasks = opts.Tasks,
            .ShowQuests = opts.Quests,
            .ShowErrors = opts.Errors,
            .ShowSelfies = opts.Selfies,
            .ChatsId = ids.ChatsId,
            .TasksId = ids.TasksId,
            .QuestsId = ids.QuestsId,
            .ErrorsId = ids.ErrorsId,
            .SelfiesId = ids.SelfiesId
        })
        Next

        Return list
    End Function

    Private Sub SaveRoutesToCfg()
        Dim path = CfgPath()
        Dim root As JObject
        If IO.File.Exists(path) Then
            root = JObject.Parse(IO.File.ReadAllText(path, System.Text.Encoding.UTF8))
        Else
            root = New JObject()
        End If

        Dim routes = CollectRoutesFromUI()
        Dim arr As New JArray()
        For Each r In routes
            arr.Add(New JObject(
            New JProperty("Account", r.Account),
            New JProperty("ForumWebhook", r.ForumWebhook),
            New JProperty("ShowChats", r.ShowChats),
            New JProperty("ShowTasks", r.ShowTasks),
            New JProperty("ShowQuests", r.ShowQuests),
            New JProperty("ShowErrors", r.ShowErrors),
            New JProperty("ShowSelfies", r.ShowSelfies),
            New JProperty("ChatsId", r.ChatsId),
            New JProperty("TasksId", r.TasksId),
            New JProperty("QuestsId", r.QuestsId),
            New JProperty("ErrorsId", r.ErrorsId),
            New JProperty("SelfiesId", r.SelfiesId)
        ))
        Next
        root("ThreadRoutes") = arr
        IO.File.WriteAllText(path, root.ToString(Formatting.Indented), System.Text.Encoding.UTF8)
    End Sub

    Private Sub LoadRoutesFromCfg()
        Dim path = CfgPath()
        If Not IO.File.Exists(path) Then Exit Sub

        Dim root = JObject.Parse(IO.File.ReadAllText(path, System.Text.Encoding.UTF8))
        Dim arr = TryCast(root("ThreadRoutes"), JArray)
        If arr Is Nothing Then Exit Sub

        Dim targetCards As New List(Of MaterialCard)
        If cards.Count > 0 Then targetCards.Add(cards(0))

        Dim need As Integer = Math.Max(0, arr.Count - targetCards.Count)
        For i = 1 To need
            Dim newCard = CreateCardClone()
            canvas.Controls.Add(newCard)
            cards.Add(newCard)
            targetCards.Add(newCard)
        Next

        selectedAccounts.Clear()

        For i = 0 To arr.Count - 1
            Dim obj = TryCast(arr(i), JObject)
            If obj Is Nothing Then Continue For

            Dim card = targetCards(i)

            Dim acctName As String = CStr(obj.Value(Of String)("Account"))
            Dim cmb = TryCast(FindChildByName(card, "accounts"), MaterialComboBox)
            If cmb IsNot Nothing Then
                RefreshOneAccountCombo(cmb)
                If Not String.IsNullOrWhiteSpace(acctName) Then
                    Dim idx = cmb.Items.IndexOf(acctName)
                    If idx >= 0 Then
                        cmb.SelectedIndex = idx
                    Else
                        cmb.SelectedIndex = -1
                    End If

                    cmb.Tag = acctName
                    selectedAccounts.Add(acctName)
                End If
            End If

            Dim tb = FindWebhookTextBox(card)
            If tb IsNot Nothing Then tb.Text = CStr(obj.Value(Of String)("ForumWebhook"))
            Dim state As New DTMState With {
                .Options = New DTMOpts With {
                    .Chats = obj.Value(Of Boolean?)("ShowChats").GetValueOrDefault(True),
                    .Tasks = obj.Value(Of Boolean?)("ShowTasks").GetValueOrDefault(True),
                    .Quests = obj.Value(Of Boolean?)("ShowQuests").GetValueOrDefault(True),
                    .Errors = obj.Value(Of Boolean?)("ShowErrors").GetValueOrDefault(True),
                    .Selfies = obj.Value(Of Boolean?)("ShowSelfies").GetValueOrDefault(True)
                },
                .Ids = New ThreadIdSet With {
                    .ChatsId = CStr(obj.Value(Of String)("ChatsId")),
                    .TasksId = CStr(obj.Value(Of String)("TasksId")),
                    .QuestsId = CStr(obj.Value(Of String)("QuestsId")),
                    .ErrorsId = CStr(obj.Value(Of String)("ErrorsId")),
                    .SelfiesId = CStr(obj.Value(Of String)("SelfiesId"))
                }
            }
            card.Tag = state

            Dim btn = TryCast(FindChildByName(card, "btnMonitorOptions"), MaterialButton)
            If btn IsNot Nothing Then btn.Text = "Monitor Options"
        Next
        RefreshAllAccountCombos()
        LayoutCards()
    End Sub

End Class
