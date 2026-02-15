namespace Task_1.Shared
{
    public partial class GuidedTour
    {
        private bool showTour;
        private int stepIndex = 0;

        private record TourStep(string Title, string Description);

        private readonly List<TourStep> steps = new()
    {
        new TourStep("Welcome to TimeSheet", "This short tour highlights the main areas: navigation, adding work, and the work grid. Click Next to continue."),
        new TourStep("Navigation", "Use the top navigation (TimeSheet) to access Home, Work, Manager Approval and Reports."),
        new TourStep("Add Work", "Click the 'Add Work' button to open the Add Work modal. Choose Project/Ticket/Learning, set times and save."),
        new TourStep("Work Grid", "Your work entries appear in the grid. Use the Edit action to update an entry. Filter by date range using the From/To controls."),
        new TourStep("Manager Approval & Reports", "Managers can review and approve entries in Manager Approval. Use the Reports menu to export or view reports.")
    };

        private TourStep CurrentStep => steps[stepIndex];

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var shown = await _localStorage.GetAsync<bool?>("TourShown");
                if (shown.Success != true)
                {
                    showTour = true;
                }
            }
            catch
            {
                // On storage error, fall back to showing the tour once
                showTour = true;
            }
        }

        private async Task NextStep()
        {
            if (stepIndex < steps.Count - 1)
            {
                stepIndex++;
            }
            else
            {
                await FinishTour();
            }
        }

        private void PrevStep()
        {
            if (stepIndex > 0)
                stepIndex--;
        }

        private async Task SkipTour()
        {
            await FinishTour();
        }

        private async Task FinishTour()
        {
            showTour = false;
            try
            {
                await _localStorage.SetAsync("TourShown", true);
            }
            catch
            {
                // swallow storage errors — tour will reappear next load if can't persist
            }
        }
    }
}
