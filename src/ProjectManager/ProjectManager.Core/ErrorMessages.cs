namespace ProjectManager.Core
{
    public static class ErrorMessages
    {
        public const string FieldUniqueErrorTemplate = "{0} should be unique";
        public const string FieldsCombinationUniqueErrorTemplate = "Combination of {0} should be unique";
        public const string FkErrorTemplate = "Can not delete {0} because is used in {1}";
    }
}