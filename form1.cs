using System;
using System.Linq;

namespace SafeSQL
{
    
    public class MainForm : System.Windows.Forms.Form
    {
        private ComboBox queryTypeDropdown;
        private Panel inputPanel;
        private TextBox previewTextBox;
        private Button copyButton;
        private Button addSetRowButton;

        private List<(TextBox column, TextBox value, CheckBox needsQuotes)> setRows;
        private TextBox tableNameBox, whereColumnBox, whereValueBox;
        private CheckBox whereNeedsQuotes;

        private Label whereLabel;
        private Button generateButton;

        public MainForm()
        {
            InitializeComponents();
            
        }
        
        private void InitializeComponents()
        {
            this.Text = "SQL Query Builder";
            this.Width = 600;
            this.Height = 600;

            queryTypeDropdown = new ComboBox { Left = 10, Top = 10, Width = 200 };
            queryTypeDropdown.Items.AddRange(new[] { "DELETE", "UPDATE" });
            queryTypeDropdown.SelectedIndexChanged += QueryTypeDropdown_SelectedIndexChanged;
            this.Controls.Add(queryTypeDropdown);

            inputPanel = new Panel { Left = 10, Top = 50, Width = 560, Height = 400, AutoScroll = true };
            this.Controls.Add(inputPanel);

            previewTextBox = new TextBox
            {
                Left = 10,
                Top = 460,
                Width = 560,
                Height = 60,
                Multiline = true,
                ReadOnly = true,

            };
            this.Controls.Add(previewTextBox);

            copyButton = new Button { Text = "Copy", Left = 500, Top = 530, Width = 70 };
            copyButton.Click += CopyButton_Click;
            this.Controls.Add(copyButton);

            setRows = new List<(TextBox, TextBox, CheckBox)>();
        }

        private void QueryTypeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            inputPanel.Controls.Clear();
            setRows.Clear();

            string selectedType = queryTypeDropdown.SelectedItem.ToString();
            if (selectedType == "DELETE")
            {
                InitializeDeleteInputs();
            }
            else if (selectedType == "UPDATE")
            {
                InitializeUpdateInputs();
            }
        }

        private void InitializeDeleteInputs()
        {
            Label tableLabel = new Label { Text = "Table Name:", Left = 10, Top = 10 };
            inputPanel.Controls.Add(tableLabel);
            tableNameBox = new TextBox { Left = 120, Top = 10, Width = 200 };
            inputPanel.Controls.Add(tableNameBox);

            whereLabel = new Label { Text = "WHERE:", Left = 10, Top = 50 };
            inputPanel.Controls.Add(whereLabel);
            whereColumnBox = new TextBox { Left = 120, Top = 50, Width = 100 };
            inputPanel.Controls.Add(whereColumnBox);

            Label equalsLabel = new Label { Text = "=", Left = 225, Top = 55, Width = 15 }; // Add equals sign
            inputPanel.Controls.Add(equalsLabel);

            whereValueBox = new TextBox { Left = 250, Top = 50, Width = 100 };
            inputPanel.Controls.Add(whereValueBox);

            whereNeedsQuotes = new CheckBox { Text = "Needs quotes?", Left = 360, Top = 50 };
            inputPanel.Controls.Add(whereNeedsQuotes);

            generateButton = new Button { Text = "Generate", Left = 10, Top = 100 };
            generateButton.Click += GenerateDeleteQuery;
            inputPanel.Controls.Add(generateButton);
        }

        private void InitializeUpdateInputs()
        {
            Label tableLabel = new Label { Text = "Table Name:", Left = 10, Top = 10 };
            inputPanel.Controls.Add(tableLabel);
            tableNameBox = new TextBox { Left = 120, Top = 10, Width = 200 };
            inputPanel.Controls.Add(tableNameBox);

            Label setLabel = new Label { Text = "SET:", Left = 10, Top = 50 };
            inputPanel.Controls.Add(setLabel);

            AddSetRow(50);

            addSetRowButton = new Button { Text = "Add Row", Left = 10, Top = 90 };
            addSetRowButton.Click += AddSetRowButton_Click;
            inputPanel.Controls.Add(addSetRowButton);

            whereLabel = new Label { Text = "WHERE:", Left = 10, Top = 130 };
            inputPanel.Controls.Add(whereLabel);
            whereColumnBox = new TextBox { Left = 120, Top = 130, Width = 100 };
            inputPanel.Controls.Add(whereColumnBox);

            Label equalsLabel = new Label { Text = "=", Left = 225, Top = 135, Width = 15 }; // Add equals sign for UPDATE WHERE
            inputPanel.Controls.Add(equalsLabel);

            whereValueBox = new TextBox { Left = 250, Top = 130, Width = 100 };
            inputPanel.Controls.Add(whereValueBox);

            whereNeedsQuotes = new CheckBox { Text = "Needs quotes?", Left = 360, Top = 130 };
            inputPanel.Controls.Add(whereNeedsQuotes);

            generateButton = new Button { Text = "Generate", Left = 10, Top = 170 };
            generateButton.Click += GenerateUpdateQuery;
            inputPanel.Controls.Add(generateButton);
        }

        private void AddSetRowButton_Click(object sender, EventArgs e)
        {
            int newTop = 50 + setRows.Count * 30;
            AddSetRow(newTop);

            // Dynamically move WHERE section and Generate button
            int whereTop = newTop + 40;
            whereLabel.Top = whereTop;
            whereColumnBox.Top = whereTop;
            whereValueBox.Top = whereTop;
            whereNeedsQuotes.Top = whereTop;

            generateButton.Top = whereTop + 40;
        }

        private void AddSetRow(int top)
        {
            TextBox columnBox = new TextBox { Left = 120, Top = top, Width = 100 };
            inputPanel.Controls.Add(columnBox);

            Label equalsLabel = new Label { Text = "=", Left = 225, Top = top + 5, Width = 15 }; // Static equals sign
            inputPanel.Controls.Add(equalsLabel);

            TextBox valueBox = new TextBox { Left = 250, Top = top, Width = 100 };
            inputPanel.Controls.Add(valueBox);

            CheckBox needsQuotesBox = new CheckBox { Text = "Needs quotes?", Left = 360, Top = top };
            inputPanel.Controls.Add(needsQuotesBox);

            setRows.Add((columnBox, valueBox, needsQuotesBox));
        }

        private void GenerateDeleteQuery(object sender, EventArgs e)
        {
            string tableName = tableNameBox.Text;
            string whereColumn = whereColumnBox.Text;
            string whereValue = whereValueBox.Text;
            string whereValueFormatted = whereNeedsQuotes.Checked ? $"'{whereValue}'" : whereValue;

            string query = $"DELETE FROM {tableName} WHERE {whereColumn} = {whereValueFormatted};";
            previewTextBox.Text = query;
        }

        private void GenerateUpdateQuery(object sender, EventArgs e)
        {
            string tableName = tableNameBox.Text;
            string setClause = string.Join(", ", setRows.Select(row =>
            {
                string valueFormatted = row.needsQuotes.Checked ? $"'{row.value.Text}'" : row.value.Text;
                return $"{row.column.Text} = {valueFormatted}";
            }));

            string whereColumn = whereColumnBox.Text;
            string whereValue = whereValueBox.Text;
            string whereValueFormatted = whereNeedsQuotes.Checked ? $"'{whereValue}'" : whereValue;

            string query = $"UPDATE {tableName} SET {setClause} WHERE {whereColumn} = {whereValueFormatted};";
            previewTextBox.Text = query;
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(previewTextBox.Text);
        }
    }
}
