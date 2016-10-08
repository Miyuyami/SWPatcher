namespace SWPatcherTest.General
{
    class ResxLanguage
    {
        public string Language { get; private set; }
        public string Code { get; private set; }

        public ResxLanguage(string language, string code)
        {
            this.Language = language;
            this.Code = code;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            ResxLanguage language = obj as ResxLanguage;
            return Language == language.Language;
        }

        public override int GetHashCode()
        {
            return Language.GetHashCode();
        }

        public override string ToString()
        {
            return Language;
        }
    }
}
