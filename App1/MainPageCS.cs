using Xamarin.Forms;

namespace App1
{
	public class MainPageCS : ContentPage
	{
		public MainPageCS ()
		{
			Title = "Main Page";
			Padding = new Thickness (0, 20, 0, 0);
			Content = new StackLayout { 
				Children = {
					new Label { Text = "Camera Preview:" },
					new CameraPreview {
						Camera = CameraOptions.Rear,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.FillAndExpand
					} 
				}
			};
		}
	}
}
