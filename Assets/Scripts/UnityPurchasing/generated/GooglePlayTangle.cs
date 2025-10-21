// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("37FFJLZebJeWRGJ43OfX0f+rdmhm5evk1Gbl7uZm5eXkAt7+Phg8IswPaZngiqJvCAdIp35w34beIM+Ke9qOOa+AkzVSYcGAGF7Idcv3XTfDXh0+GOt8W3/YpjMqU5NfRpvnzB2Op5Urdvn9k0jkJchy9hRUff5js4rE6IUto/iVJMo+Xik3Z6x1uq1lZ4aNESlsSnE0rj537kZO0FsWidRm5cbU6eLtzmKsYhPp5eXl4eTnhTEmfoWVZqxqABJ9RzMTZY6cK1cWajLykPPJuSPhpR9ENy2s4SUVbjtanihfyA5Szhf7JvEI93q0oe/iklNS1W1TJt0MgnfZmwC6s8lF8vATjzniJRbXSKuMYLvNmTU6P80B49NRDNY98iOe5ebn5eTl");
        private static int[] order = new int[] { 3,1,13,8,11,9,6,12,8,9,11,13,13,13,14 };
        private static int key = 228;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
