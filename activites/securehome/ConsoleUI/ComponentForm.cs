using System.Windows.Forms;

namespace ConsoleUI;

/// <summary>
/// Simple form with output log and input textbox
/// </summary>
public class ComponentForm : Form
{
    private TextBox _logTextBox;
    private TextBox _inputTextBox;
    private Button _sendButton;
    private Label _statusLabel;

    public Action<string>? OnCommand { get; set; }

    public ComponentForm(string title)
    {
        Text = title;
        Size = new System.Drawing.Size(600, 500);

        // Log TextBox (multiline, read-only)
        _logTextBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill,
            Font = new System.Drawing.Font("Consolas", 9),
            BackColor = System.Drawing.Color.Black,
            ForeColor = System.Drawing.Color.LightGreen
        };

        // Input Panel
        var inputPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60
        };

        // Status Label
        _statusLabel = new Label
        {
            Text = "Ready",
            Dock = DockStyle.Top,
            Height = 20,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        };

        // Input TextBox
        _inputTextBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new System.Drawing.Font("Consolas", 10)
        };

        _inputTextBox.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendCommand();
            }
        };

        // Send Button
        _sendButton = new Button
        {
            Text = "Send",
            Dock = DockStyle.Right,
            Width = 80
        };

        _sendButton.Click += (s, e) => SendCommand();

        // Assemble
        inputPanel.Controls.Add(_inputTextBox);
        inputPanel.Controls.Add(_sendButton);
        inputPanel.Controls.Add(_statusLabel);

        Controls.Add(_logTextBox);
        Controls.Add(inputPanel);
    }

    private void SendCommand()
    {
        var command = _inputTextBox.Text.Trim();
        if (!string.IsNullOrEmpty(command))
        {
            WriteLog($"> {command}");
            OnCommand?.Invoke(command);
            _inputTextBox.Clear();
        }
    }

    public void WriteLog(string message)
    {
        if (!IsHandleCreated)
            return; // Form not ready yet

        if (InvokeRequired)
        {
            try
            {
                BeginInvoke(new Action<string>(WriteLog), message);
            }
            catch
            {
                // Ignore if form is closing/disposed
            }
            return;
        }

        try
        {
            string msg = $"[{DateTime.Now:HH:mm:ss}] {message}\r\n";
            _logTextBox.AppendText(msg);
            File.AppendAllText("log.txt", $"{this.Text}\t");
            File.AppendAllText("log.txt", msg);
        }
        catch
        {
            // Ignore if control is disposed
        }
    }

    public void SetStatus(string status)
    {
        if (!IsHandleCreated)
            return;

        if (InvokeRequired)
        {
            try
            {
                BeginInvoke(new Action<string>(SetStatus), status);
            }
            catch
            {
                // Ignore if form is closing/disposed
            }
            return;
        }

        try
        {
            _statusLabel.Text = status;
        }
        catch
        {
            // Ignore if control is disposed
        }
    }
}
