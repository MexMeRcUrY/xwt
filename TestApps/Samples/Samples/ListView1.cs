using System;
using Xwt;
using Xwt.Drawing;

namespace Samples
{
	public class ListView1: VBox
	{
		DataField<string> name = new DataField<string> ();
		DataField<Image> icon = new DataField<Image> ();
		DataField<string> text = new DataField<string> ();
		DataField<Image> icon2 = new DataField<Image> ();
		DataField<CellData> progress = new DataField<CellData> ();

		public ListView1 ()
		{
			PackStart (new Label ("The listview should have a red background"));
			ListView list = new ListView ();
			ListStore store = new ListStore (name, icon, text, icon2, progress);
			list.DataSource = store;
			list.Columns.Add ("Name", icon, name);
			list.Columns.Add ("Text", icon2, text);
			list.Columns.Add ("Progress", new TextCellView () { TextField = text }, new CustomCell () { ValueField = progress });

			var png = Image.FromResource (typeof(App), "class.png");

			Random rand = new Random ();
			
			for (int n=0; n<100; n++) {
				var r = store.AddRow ();
				store.SetValue (r, icon, png);
				store.SetValue (r, name, "Value " + n);
				store.SetValue (r, icon2, png);
				store.SetValue (r, text, "Text " + n);
				store.SetValue (r, progress, new CellData { Value = rand.Next () % 100 });
			}
			PackStart (list, true);

			list.RowActivated += delegate(object sender, ListViewRowEventArgs e) {
				MessageDialog.ShowMessage ("Row " + e.RowIndex + " activated");
			};

			var but = new Button ("Scroll one line");
			but.Clicked += delegate {
				list.VerticalScrollControl.Value += list.VerticalScrollControl.StepIncrement;
			};
			PackStart (but);
		}
	}

	class CellData
	{
		public int Value;
		public double YPos = -1;
	}

	class CustomCell: CanvasCellView
	{
		public IDataField<CellData> ValueField { get; set; }

		protected override Size OnGetRequiredSize ()
		{
			return new Size (200, 10);
		}

		protected override void OnDraw (Context ctx, Rectangle cellArea)
		{
			ctx.Rectangle (BackgroundBounds);
			ctx.SetColor (new Color (0.9, 0.9, 0.9));
			ctx.Fill ();

			ctx.Rectangle (Bounds);
			ctx.SetColor (new Color (0.7, 0.7, 0.7));
			ctx.Fill ();

			var pct = GetValue (ValueField);
			var size = (cellArea.Width * pct.Value) / 100f;
			cellArea.Width = (int) size;

			ctx.SetLineWidth (1);
			ctx.Rectangle (cellArea.Inflate (-0.5, -0.5));
			ctx.SetColor (Selected ? Colors.Blue : Colors.LightBlue);
			ctx.FillPreserve ();
			ctx.SetColor (Colors.Gray);
			ctx.Stroke ();

			if (pct.YPos != -1) {
				ctx.MoveTo (cellArea.Right, Bounds.Y + pct.YPos);
				ctx.Arc (cellArea.Right, Bounds.Y + pct.YPos, 2.5, 0, 360);
				ctx.SetColor (Colors.Red);
				ctx.Fill ();
			}
		}

		protected override void OnMouseMoved (MouseMovedEventArgs args)
		{
			var data = GetValue (ValueField);
			data.Value = (int) (100 * ((args.X - Bounds.X) / Bounds.Width));
			data.YPos = args.Y - Bounds.Y;
			QueueDraw ();
			base.OnMouseMoved (args);
		}

		protected override void OnButtonPressed (ButtonEventArgs args)
		{
			Console.WriteLine ("Press: " + args.Position);
			base.OnButtonPressed (args);
		}

		protected override void OnButtonReleased (ButtonEventArgs args)
		{
			Console.WriteLine ("Release: " + args.Position);
			base.OnButtonReleased (args);
		}
	}
}

