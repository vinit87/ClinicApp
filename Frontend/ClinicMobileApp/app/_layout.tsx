import { Stack } from 'expo-router';
import { StatusBar } from 'expo-status-bar';
import { AuthProvider } from '../context/AuthContext';

export default function RootLayout() {
  return (
    <AuthProvider>
      <StatusBar style="light" />
      <Stack
        screenOptions={{
          headerStyle: {
            backgroundColor: '#0284c7',
          },
          headerTintColor: '#ffffff',
          headerTitleStyle: {
            fontWeight: 'bold',
          },
        }}
      >
        <Stack.Screen
          name="index"
          options={{
            title: 'Clinic Appointment',
            headerShown: true,
          }}
        />
        <Stack.Screen
          name="appointments"
          options={{
            title: 'Appointments Dashboard',
            headerShown: true,
          }}
        />
        <Stack.Screen
          name="book"
          options={{
            title: 'Book Appointment',
            headerShown: true,
          }}
        />
      </Stack>
    </AuthProvider>
  );
}
