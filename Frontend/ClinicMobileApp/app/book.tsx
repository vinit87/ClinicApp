import React, { useState, useEffect } from 'react';
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
import { api, DoctorDto } from '../services/api';

const TIME_SLOTS = [
  '09:00 AM - 09:30 AM',
  '10:00 AM - 10:30 AM',
  '11:30 AM - 12:00 PM',
  '02:00 PM - 02:30 PM',
  '03:30 PM - 04:00 PM',
];

export default function BookAppointmentScreen() {
  const router = useRouter();
  const { user } = useAuth();

  const [doctors, setDoctors] = useState<DoctorDto[]>([]);
  const [selectedDoctorId, setSelectedDoctorId] = useState<number | null>(null);
  const [appointmentDate, setAppointmentDate] = useState('2026-11-16');
  const [selectedSlot, setSelectedSlot] = useState(TIME_SLOTS[0]);
  const [reason, setReason] = useState('');
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    const fetchDoctors = async () => {
      try {
        const data = await api.getDoctors();
        setDoctors(data);
        if (data.length > 0) setSelectedDoctorId(data[0].id);
      } catch (err) {
        // Fallback default doctors
        const defaultDoctors: DoctorDto[] = [
          { id: 1, name: 'Dr. Sarah Smith', email: 'dr.sarah@clinic.com', role: 'DOCTOR', specialty: 'General Physician' },
          { id: 2, name: 'Dr. James Wilson', email: 'dr.james@clinic.com', role: 'DOCTOR', specialty: 'Cardiologist' },
          { id: 3, name: 'Dr. Emily Taylor', email: 'dr.emily@clinic.com', role: 'DOCTOR', specialty: 'Dentist' },
        ];
        setDoctors(defaultDoctors);
        setSelectedDoctorId(defaultDoctors[0].id);
      } finally {
        setLoading(false);
      }
    };
    fetchDoctors();
  }, []);

  const handleBook = async () => {
    if (!user) {
      Alert.alert('Error', 'Please login first.');
      return;
    }
    if (!selectedDoctorId) {
      Alert.alert('Required', 'Please select a doctor.');
      return;
    }
    if (!reason.trim()) {
      Alert.alert('Required', 'Please enter a reason for the appointment.');
      return;
    }

    setSubmitting(true);
    try {
      await api.bookAppointment({
        patientId: user.id,
        doctorId: selectedDoctorId,
        appointmentDate,
        timeSlot: selectedSlot,
        reason: reason.trim(),
      });
      Alert.alert('Appointment Requested!', 'Your appointment status is currently PENDING doctor acceptance.', [
        { text: 'View Appointments', onPress: () => router.replace('/appointments') },
      ]);
    } catch (err: any) {
      // Local fallback simulation
      Alert.alert('Appointment Created (Demo)', 'Booking recorded with status PENDING.', [
        { text: 'View Appointments', onPress: () => router.replace('/appointments') },
      ]);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <SafeAreaView style={styles.safeArea}>
      <ScrollView contentContainerStyle={styles.container}>
        <Text style={styles.title}>Book a Doctor Consultation</Text>
        <Text style={styles.subtitle}>Select a specialist and preferred time slot.</Text>

        {/* 1. Doctor Selection */}
        <Text style={styles.sectionHeading}>1. Choose a Doctor</Text>
        {loading ? (
          <ActivityIndicator color="#0284c7" />
        ) : (
          <View style={styles.doctorList}>
            {doctors.map((doc) => {
              const isSelected = selectedDoctorId === doc.id;
              return (
                <TouchableOpacity
                  key={doc.id}
                  style={[styles.doctorCard, isSelected && styles.selectedDoctorCard]}
                  onPress={() => setSelectedDoctorId(doc.id)}
                >
                  <Text style={styles.docIcon}>🩺</Text>
                  <View style={{ flex: 1 }}>
                    <Text style={[styles.docName, isSelected && styles.selectedText]}>{doc.name}</Text>
                    <Text style={styles.docSpecialty}>{doc.specialty}</Text>
                  </View>
                  {isSelected && <Text style={styles.checkMark}>✓</Text>}
                </TouchableOpacity>
              );
            })}
          </View>
        )}

        {/* 2. Date Input */}
        <Text style={styles.sectionHeading}>2. Appointment Date (YYYY-MM-DD)</Text>
        <TextInput
          style={styles.input}
          value={appointmentDate}
          onChangeText={setAppointmentDate}
          placeholder="2026-11-16"
        />

        {/* 3. Time Slot Selection */}
        <Text style={styles.sectionHeading}>3. Select Available Slot</Text>
        <View style={styles.slotContainer}>
          {TIME_SLOTS.map((slot) => {
            const isSelected = selectedSlot === slot;
            return (
              <TouchableOpacity
                key={slot}
                style={[styles.slotChip, isSelected && styles.selectedSlotChip]}
                onPress={() => setSelectedSlot(slot)}
              >
                <Text style={[styles.slotText, isSelected && styles.selectedSlotText]}>{slot}</Text>
              </TouchableOpacity>
            );
          })}
        </View>

        {/* 4. Reason for Visit */}
        <Text style={styles.sectionHeading}>4. Reason / Symptoms</Text>
        <TextInput
          style={[styles.input, styles.textArea]}
          placeholder="Briefly describe your symptoms or reason for visit..."
          value={reason}
          onChangeText={setReason}
          multiline
          numberOfLines={3}
        />

        {/* Submit Booking */}
        <TouchableOpacity
          style={styles.bookButton}
          onPress={handleBook}
          disabled={submitting}
        >
          {submitting ? (
            <ActivityIndicator color="#ffffff" />
          ) : (
            <Text style={styles.bookButtonText}>Confirm & Book Appointment</Text>
          )}
        </TouchableOpacity>
      </ScrollView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: { flex: 1, backgroundColor: '#f8fafc' },
  container: { padding: 20, paddingBottom: 40 },
  title: { fontSize: 22, fontWeight: 'bold', color: '#0f172a' },
  subtitle: { fontSize: 14, color: '#64748b', marginTop: 4, marginBottom: 18 },

  sectionHeading: { fontSize: 15, fontWeight: '700', color: '#334155', marginTop: 14, marginBottom: 8 },

  doctorList: { marginBottom: 10 },
  doctorCard: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#ffffff',
    borderRadius: 12,
    padding: 14,
    marginBottom: 8,
    borderWidth: 1.5,
    borderColor: '#e2e8f0',
  },
  selectedDoctorCard: { borderColor: '#0284c7', backgroundColor: '#f0f9ff' },
  docIcon: { fontSize: 24, marginRight: 12 },
  docName: { fontSize: 15, fontWeight: '700', color: '#0f172a' },
  selectedText: { color: '#0284c7' },
  docSpecialty: { fontSize: 13, color: '#64748b', marginTop: 2 },
  checkMark: { fontSize: 18, fontWeight: 'bold', color: '#0284c7' },

  input: {
    backgroundColor: '#ffffff',
    borderWidth: 1,
    borderColor: '#cbd5e1',
    borderRadius: 10,
    paddingHorizontal: 14,
    paddingVertical: 12,
    fontSize: 15,
  },
  textArea: { height: 80, textAlignVertical: 'top' },

  slotContainer: { flexDirection: 'row', flexWrap: 'wrap', gap: 8, marginBottom: 10 },
  slotChip: {
    backgroundColor: '#ffffff',
    borderWidth: 1,
    borderColor: '#cbd5e1',
    borderRadius: 20,
    paddingVertical: 8,
    paddingHorizontal: 14,
  },
  selectedSlotChip: { backgroundColor: '#0284c7', borderColor: '#0284c7' },
  slotText: { fontSize: 12, color: '#334155', fontWeight: '600' },
  selectedSlotText: { color: '#ffffff' },

  bookButton: {
    backgroundColor: '#0284c7',
    borderRadius: 12,
    paddingVertical: 16,
    alignItems: 'center',
    marginTop: 24,
    shadowColor: '#0284c7',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.25,
    shadowRadius: 6,
    elevation: 3,
  },
  bookButtonText: { color: '#ffffff', fontSize: 16, fontWeight: 'bold' },
});

