namespace Percas
{
    public static class ServiceLocator
    {
        public static PopupController PopupScene { get; private set; }
        public static PopupController PopupGlobal { get; private set; }

        public static void RegisterPopupScene(PopupController controller)
        {
            PopupScene = controller;
        }

        public static void RegisterPopupGlobal(PopupController controller)
        {
            PopupGlobal = controller;
        }
    }
}
