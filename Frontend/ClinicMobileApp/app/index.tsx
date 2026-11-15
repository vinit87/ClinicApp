import React, { useState } from 'react';
import {
  StyleSheet,
  Text,
  View,
  TextInput,
  TouchableOpacity,
  ScrollView,
  SafeAreaView,
  ActivityIndicator,
  Alert,
} from 'react-native';
import { useRouter } from 'expo-router';
import { useAuth } from '../context/AuthContext';
import { api, getApiBaseUrl, setApiBaseUrl } from '../services/api';

export default function LoginScreen() {
  const router = useRouter();
  const { login } = useAuth();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [apiUrl, setApiUrl] = useState(getApiBaseUrl());
  const [showConfig, setShowConfig] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleLogin = async (loginEmail?: string, loginPassword?: string) => {
    const targetEmail = loginEmail || email;
    const targetPassword = loginPassword || password;

    if (!targetEmail || !targetPassword) {
      Alert.alert('Required', 'Please enter email and password.');
      return;
    }

    setLoading(true);
    try {
      setApiBaseUrl(apiUrl);
      const user = await api.login(targetEmail, targetPassword);
      login(user);
      router.push('/appointments');
    } catch (err: any) {
      // If backend is not reached yet or database password is not configured on PC,
      // fallback to offline demo mode so the user can still test the UI on their phone!
      Alert.alert(
        'Backend Notice',
        `${err.message || 'Could not connect to backend'}.\n\nWould you like to enter in Demo Mode for UI testing?`,
        [
          { text: 'Cancel', style: 'cancel' },
          {
            text: 'Enter Demo Mode',
            onPress: () => {
              const isDoctor = targetEmail.includes('dr.') || targetEmail.includes('doctor');
              const demoUser = {
                id: isDoctor ? 1 : 4,
                name: isDoctor ? 'Dr. Sarah Smith' : 'Alex Johnson',
                email: targetEmail,
                role: (isDoctor ? 'DOCTOR' : 'PATIENT') as 'DOCTOR' | 'PATIENT',
                specialty: isDoctor ? 'General Physician' : undefined,
              };
              login(demoUser);
              router.push('/appointments');
            },
          },
        ]
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <SafeAreaView style={styles.safeArea}>
      <ScrollView contentContainerStyle={styles.container}>
        {/* Brand Header */}
        <View style={styles.header}>
          <Text style={styles.logoIcon}>🏥</Text>
          <Text style={styles.appTitle}>Clinic Appointment</Text>
          <Text style={styles.tagline}>Healthcare Booking & Doctor Portal</Text>
        </View>

        {/* Quick Demo Logins Card */}
        <View style={styles.quickCard}>
          <Text style={styles.quickTitle}>⚡ Quick 1-Tap Login (Demo Accounts)</Text>
          <Text style={styles.quickSubtitle}>Select a role to test on your phone instantly:</Text>

          <TouchableOpacity
            style={[styles.quickButton, styles.patientBtn]}
            onPress={() => handleLogin('alex@patient.com', 'patient123')}
          >
            <Text style={styles.quickBtnText}>👤 Login as Patient (Alex Johnson)</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={[styles.quickButton, styles.doctorBtn]}
            onPress={() => handleLogin('dr.sarah@clinic.com', 'doctor123')}
          >
            <Text style={styles.quickBtnText}>🩺 Login as Doctor (Dr. Sarah Smith)</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={[styles.quickButton, styles.doctorBtn]}
            onPress={() => handleLogin('dr.james@clinic.com', 'doctor123')}
          >
            <Text style={styles.quickBtnText}>❤️ Login as Doctor (Dr. James Wilson)</Text>
          </TouchableOpacity>
        </View>

        {/* Standard Credentials Input */}
        <View style={styles.formCard}>
          <Text style={styles.formTitle}>Or Enter Credentials</Text>

          <Text style={styles.inputLabel}>Email Address</Text>
          <TextInput
            style={styles.input}
            placeholder="e.g. alex@patient.com"
            value={email}
            onChangeText={setEmail}
            autoCapitalize="none"
            keyboardType="email-address"
          />

          <Text style={styles.inputLabel}>Password</Text>
          <TextInput
            style={styles.input}
            placeholder="••••••••"
            value={password}
            onChangeText={setPassword}
            secureTextEntry
          />

          <TouchableOpacity
            style={styles.loginButton}
            onPress={() => handleLogin()}
            disabled={loading}
          >
            {loading ? (
              <ActivityIndicator color="#ffffff" />
            ) : (
              <Text style={styles.loginButtonText}>Sign In</Text>
            )}
          </TouchableOpacity>
        </View>

        {/* Network & Backend Configuration Toggle */}
        <TouchableOpacity
          style={styles.configToggle}
          onPress={() => setShowConfig(!showConfig)}
        >
          <Text style={styles.configToggleText}>
            ⚙️ {showConfig ? 'Hide API URL Settings' : 'Configure Backend API URL'}
          </Text>
        </TouchableOpacity>

        {showConfig && (
          <View style={styles.configBox}>
            <Text style={styles.configLabel}>Backend API URL:</Text>
            <TextInput
              style={styles.configInput}
              value={apiUrl}
              onChangeText={setApiUrl}
              placeholder="http://<your-pc-ip>:5290/api"
              autoCapitalize="none"
            />
            <Text style={styles.configHelp}>
              Tip: When using Expo Go on phone, use your PC's Wi-Fi IP instead of localhost.
            </Text>
          </View>
        )}
      </ScrollView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: { flex: 1, backgroundColor: '#f8fafc' },
  container: { padding: 20, paddingBottom: 40 },
  header: { alignItems: 'center', marginTop: 10, marginBottom: 24 },
  logoIcon: { fontSize: 44, marginBottom: 8 },
  appTitle: { fontSize: 26, fontWeight: 'bold', color: '#0f172a' },
  tagline: { fontSize: 14, color: '#64748b', marginTop: 4 },

  quickCard: {
    backgroundColor: '#ffffff',
    borderRadius: 16,
    padding: 18,
    marginBottom: 20,
    borderWidth: 1,
    borderColor: '#e2e8f0',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 6,
    elevation: 2,
  },
  quickTitle: { fontSize: 15, fontWeight: '700', color: '#0284c7', marginBottom: 4 },
  quickSubtitle: { fontSize: 12, color: '#64748b', marginBottom: 12 },
  quickButton: {
    paddingVertical: 12,
    paddingHorizontal: 14,
    borderRadius: 10,
    marginBottom: 8,
  },
  patientBtn: { backgroundColor: '#e0f2fe' },
  doctorBtn: { backgroundColor: '#f0fdf4' },
  quickBtnText: { fontSize: 14, fontWeight: '600', color: '#0f172a' },

  formCard: {
    backgroundColor: '#ffffff',
    borderRadius: 16,
    padding: 20,
    marginBottom: 16,
    borderWidth: 1,
    borderColor: '#e2e8f0',
  },
  formTitle: { fontSize: 16, fontWeight: '700', color: '#1e293b', marginBottom: 14 },
  inputLabel: { fontSize: 13, fontWeight: '600', color: '#475569', marginBottom: 6 },
  input: {
    backgroundColor: '#f8fafc',
    borderWidth: 1,
    borderColor: '#cbd5e1',
    borderRadius: 10,
    paddingHorizontal: 14,
    paddingVertical: 10,
    fontSize: 15,
    marginBottom: 14,
  },
  loginButton: {
    backgroundColor: '#0284c7',
    borderRadius: 10,
    paddingVertical: 14,
    alignItems: 'center',
    marginTop: 6,
  },
  loginButtonText: { color: '#ffffff', fontSize: 16, fontWeight: 'bold' },

  configToggle: { alignSelf: 'center', marginVertical: 10 },
  configToggleText: { fontSize: 13, color: '#0284c7', fontWeight: '600' },
  configBox: {
    backgroundColor: '#f1f5f9',
    borderRadius: 10,
    padding: 14,
    marginTop: 8,
  },
  configLabel: { fontSize: 12, fontWeight: '600', color: '#334155', marginBottom: 4 },
  configInput: {
    backgroundColor: '#ffffff',
    borderWidth: 1,
    borderColor: '#cbd5e1',
    borderRadius: 8,
    padding: 8,
    fontSize: 13,
    marginBottom: 4,
  },
  configHelp: { fontSize: 11, color: '#64748b' },
});
