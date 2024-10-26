public interface IRenderer
{
    void RegisterView(IView view);
    void RegisterView(IView view, IViewModel viewModel);
    void Render();
    void Render(object viewModel);
    void Render(IViewModel viewModel);
    void RenderComposite(IView model);
    void RenderComposite(IView model, int? w, int? h);
    void RenderComposite(IView model, IViewModel viewModel);
    void RenderComposite(IView model, IViewModel viewModel, int? w, int? h);
    void RenderCompostie(IViewModel model);
    void RenderCompostie(IViewModel model, int? w, int? h);
}
