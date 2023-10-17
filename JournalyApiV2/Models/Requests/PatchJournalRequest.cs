namespace JournalyApiV2.Models.Requests;

public class PatchJournalRequest
{
    public CategoryPatch[] Categories { get; set; } = Array.Empty<CategoryPatch>();
    public EmotionPatch[] Emotions { get; set; } = Array.Empty<EmotionPatch>();
    public ActivityPatch[] Activities { get; set; } = Array.Empty<ActivityPatch>();
    public JournalPatch[] JournalEntries { get; set; } = Array.Empty<JournalPatch>();
    
    public class CategoryPatch
    {
        public Guid Uuid { get; set; }
        public string Name { get; set; }
        public short? Order { get; set; }
        public bool Deleted { get; set; } = false;
        public bool? AllowMultiple { get; set; }
        public bool Default { get; set; } = false;
    }
    public class EmotionPatch
    {
        public Guid Uuid { get; set; } 
        public string Name { get; set; }
        public Guid? Category { get; set; }
        public string? Icon { get; set; }
        public string? IconType { get; set; }
        public short? Order { get; set; }
        public bool Deleted { get; set; } = false;
    }

    public class ActivityPatch
    {
        public Guid Uuid { get; set; }
        public string Name { get; set; }
        public short? Order { get; set; }
        public string Icon { get; set; }
        public string IconType { get; set; }
        public bool Deleted { get; set; } = false;
    }

    public class JournalPatch
    {
        public class JournalPatchCategoryValue
        {
            public Guid Uuid { get; set; } // UUID of the category
            public double Value { get; set; }
        }
        public Guid Uuid { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid[] Emotions { get; set; }
        public Guid[] Activities { get; set; }
        public bool Deleted { get; set; } = false;
        public List<JournalPatchCategoryValue> CategoryValues { get; set; } = new();
    }
}