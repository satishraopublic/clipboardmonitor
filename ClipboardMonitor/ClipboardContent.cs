namespace ClipboardMonitor
{
    public class ClipboardContent
    {
        private object _content;
        public object Content
        {
            get { return _content; }
            set { _content = value; }
        }

        private string _contentType;
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }


        public ClipboardContent(string contentType, object content)
        {
            _contentType = contentType;
            _content = content;
        }
    }
}