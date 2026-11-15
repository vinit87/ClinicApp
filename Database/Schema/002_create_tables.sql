-- Phase 2: Schema Tables and Demo Seed Data
-- Schema: clinic

-- 1. Users Table (Doctors and Patients)
CREATE TABLE IF NOT EXISTS clinic.users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    password VARCHAR(100) NOT NULL,
    role VARCHAR(20) NOT NULL CHECK (role IN ('PATIENT', 'DOCTOR')),
    specialty VARCHAR(100), -- populated for doctors
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 2. Appointments Table
CREATE TABLE IF NOT EXISTS clinic.appointments (
    id SERIAL PRIMARY KEY,
    patient_id INT NOT NULL REFERENCES clinic.users(id) ON DELETE CASCADE,
    doctor_id INT NOT NULL REFERENCES clinic.users(id) ON DELETE CASCADE,
    appointment_date DATE NOT NULL,
    time_slot VARCHAR(50) NOT NULL,
    reason TEXT NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'PENDING' CHECK (status IN ('PENDING', 'ACCEPTED', 'REJECTED', 'CANCELLED')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 3. Seed Demo Doctors and Patients (Insert only if not exists)
INSERT INTO clinic.users (name, email, password, role, specialty)
VALUES 
    ('Dr. Sarah Smith', 'dr.sarah@clinic.com', 'doctor123', 'DOCTOR', 'General Physician'),
    ('Dr. James Wilson', 'dr.james@clinic.com', 'doctor123', 'DOCTOR', 'Cardiologist'),
    ('Dr. Emily Taylor', 'dr.emily@clinic.com', 'doctor123', 'DOCTOR', 'Dentist'),
    ('Alex Johnson', 'alex@patient.com', 'patient123', 'PATIENT', NULL),
    ('Maria Garcia', 'maria@patient.com', 'patient123', 'PATIENT', NULL)
ON CONFLICT (email) DO NOTHING;

-- 4. Seed Initial Appointment Demo Data
INSERT INTO clinic.appointments (patient_id, doctor_id, appointment_date, time_slot, reason, status)
SELECT p.id, d.id, CURRENT_DATE + INTERVAL '1 day', '10:00 AM', 'Routine consultation and wellness checkup', 'PENDING'
FROM clinic.users p, clinic.users d
WHERE p.email = 'alex@patient.com' AND d.email = 'dr.sarah@clinic.com'
AND NOT EXISTS (
    SELECT 1 FROM clinic.appointments 
    WHERE patient_id = p.id AND doctor_id = d.id AND reason = 'Routine consultation and wellness checkup'
);
