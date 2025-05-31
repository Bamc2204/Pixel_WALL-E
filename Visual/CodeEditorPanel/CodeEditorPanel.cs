using System.Windows.Forms;
using System.Drawing;
public class CodeEditorPanel : Panel
{
    private TextBox _textBox;

    public CodeEditorPanel()
    {
        _textBox = new TextBox
        {
            Multiline = true,
            Dock = DockStyle.Fill,
            ScrollBars = ScrollBars.Both,
            Font = new Font("Consolas", 10),
            AcceptsTab = true
        };
        Controls.Add(_textBox);
    }

    public string GetCode() => _textBox.Text;

    public void SetCode(string code) => _textBox.Text = code;
}
