// API Client for Clinic Appointment Backend

// NOTE: For testing on an Android device via Expo Go, replace localhost with your computer's local Wi-Fi IP (e.g., http://192.168.1.X:5290/api)
let API_BASE_URL = 'http://192.168.1.66:5290/api';

export const setApiBaseUrl = (url: string) => {
  API_BASE_URL = url;
};

export const getApiBaseUrl = () => API_BASE_URL;

export interface UserDto {
  id: number;
  name: string;
  email: string;
  role: 'PATIENT' | 'DOCTOR';
  specialty?: string;
}

export interface DoctorDto {
  id: number;
  name: string;
  email: string;
  role: string;
  specialty: string;
}

export interface AppointmentDto {
  id: number;
  patientId: number;
  patientName: string;
  patientEmail: string;
  doctorId: number;
  doctorName: string;
  doctorSpecialty?: string;
  appointmentDate: string;
  timeSlot: string;
  reason: string;
  status: 'PENDING' | 'ACCEPTED' | 'REJECTED' | 'CANCELLED';
  createdAt: string;
}

export const api = {
  // Auth
  login: async (email: string, password: string): Promise<UserDto> => {
    const res = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password }),
    });
    if (!res.ok) {
      const err = await res.json().catch(() => ({ message: 'Login failed' }));
      throw new Error(err.message || 'Login failed');
    }
    return res.json();
  },

  // Doctors
  getDoctors: async (): Promise<DoctorDto[]> => {
    const res = await fetch(`${API_BASE_URL}/auth/doctors`);
    if (!res.ok) throw new Error('Failed to fetch doctors');
    return res.json();
  },

  // Book appointment
  bookAppointment: async (data: {
    patientId: number;
    doctorId: number;
    appointmentDate: string;
    timeSlot: string;
    reason: string;
  }) => {
    const res = await fetch(`${API_BASE_URL}/appointments`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    if (!res.ok) {
      const err = await res.json().catch(() => ({ message: 'Booking failed' }));
      throw new Error(err.message || 'Failed to book appointment');
    }
    return res.json();
  },

  // Get patient appointments
  getPatientAppointments: async (patientId: number): Promise<AppointmentDto[]> => {
    const res = await fetch(`${API_BASE_URL}/appointments/patient/${patientId}`);
    if (!res.ok) throw new Error('Failed to fetch appointments');
    return res.json();
  },

  // Get doctor appointments
  getDoctorAppointments: async (doctorId: number): Promise<AppointmentDto[]> => {
    const res = await fetch(`${API_BASE_URL}/appointments/doctor/${doctorId}`);
    if (!res.ok) throw new Error('Failed to fetch appointments');
    return res.json();
  },

  // Update appointment status (Doctor: ACCEPTED/REJECTED, Patient: CANCELLED)
  updateStatus: async (appointmentId: number, status: 'ACCEPTED' | 'REJECTED' | 'CANCELLED') => {
    const res = await fetch(`${API_BASE_URL}/appointments/${appointmentId}/status`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ status }),
    });
    if (!res.ok) {
      const err = await res.json().catch(() => ({ message: 'Update failed' }));
      throw new Error(err.message || 'Failed to update status');
    }
    return res.json();
  },
};
