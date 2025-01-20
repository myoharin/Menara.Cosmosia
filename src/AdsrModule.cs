using System;
namespace Menara {
    public class AdsrModule {
        public float AttackTime = 0.01f; // seconds
		public float DecayTime = 0.5f; // seconds
		public float ReleaseTime = 1; // seconds

		public byte AttackIntensity = 60; // dB
		public byte SustainIntensity = 40; // dB

        public AdsrModule() {}

        public byte AdsrInterpolate(double duration, bool isHeld) {
            if (isHeld) {
                if (duration > AttackTime + DecayTime) {
                    return _sustainLerp(SustainIntensity, SustainIntensity, duration - AttackTime - DecayTime);
                }
                else if (duration > AttackTime) {
                    return _decayLerp(AttackIntensity, SustainIntensity, duration - AttackTime);
                }
                else {
                    return _attackLerp(0, AttackIntensity, duration);
                }
            }
            else {
                return _releaseLerp(SustainIntensity, 0, duration - AttackTime);
            }
        }


		public static byte _attackLerp(byte a, byte b, double t) {
			t = Math.Clamp(t, 0, 1);
			return (byte)(a + (b - a) * t);
		}
		public static byte _decayLerp(byte a, byte b, double t) {
			t = Math.Clamp(t, 0, 1);
			return (byte)(a + (b - a) * t);
		}
		public static byte _sustainLerp(byte a, byte b, double t) {
			t = Math.Clamp(t, 0, 1);
			return (byte)(a + (b - a) * t);
		}
		public static byte _releaseLerp(byte a, byte b, double t) {
			t = Math.Clamp(t, 0, 1);
			return (byte)(a + (b - a) * t);
		}
    }
        
}