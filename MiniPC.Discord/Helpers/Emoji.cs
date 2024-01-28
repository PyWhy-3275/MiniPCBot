namespace MiniPC.Discord.Helpers
{
    public static class Emoji
    {
        //Faces
        public static string MiddleFinger => ":middle_finger:";
        public static string Sunglasses => ":sunglasses:";
        public static string Cry => ":cry:";
        public static string Wink => ":wink:";
        //Music
        public static string VolumeUp => ":loud_sound:";
        public static string VolumeDown => ":sound:";
        public static string Mute => ":mute:";
        public static string Pause => ":pause_button:";
        public static string Play => ":arrow_forward:";
        public static string Notes => ":notes:";
        public static string FastForward => ":fast_forward:";
        //Off
        public static string Stop => ":no_entry:";
        public static string Info => ":information_source:";
        public static string Warning => ":warning:";
        //Test
        public static string Test() => "RETURNED STRING";
        //(string face) emoji to choose face
        public static string ScrewYou() => $"{MiddleFinger}{Sunglasses}{MiddleFinger}";
    }
}