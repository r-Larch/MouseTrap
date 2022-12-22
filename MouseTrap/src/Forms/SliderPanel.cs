namespace MouseTrap.Forms {
    public class SliderPanel : Panel {
        public List<EdgeSlider> Sliders = new List<EdgeSlider>();

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            foreach (var slider in Sliders) {
                slider.OnPaintBackground(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            foreach (var slider in Sliders) {
                slider.OnPaint(e);
            }
        }
    }
}
