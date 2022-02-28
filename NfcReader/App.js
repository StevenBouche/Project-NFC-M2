import * as React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { StyleSheet, View } from 'react-native';

import { HomeScreen } from './pages/HomeScreen';
import { ReadTagPage } from './pages/Read';
import { WriteTagPage } from './pages/Write';


const Stack = createNativeStackNavigator();


const App = () => {
  return (
    <NavigationContainer style={styles.container}>
        <Stack.Navigator initialRouteName="Home">
          <Stack.Screen name="Home" component={HomeScreen} />
          <Stack.Screen name="ReadTag" component={ReadTagPage} />
          <Stack.Screen name="WriteTag" component={WriteTagPage} />
        </Stack.Navigator>
    </NavigationContainer>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
    alignItems: 'center',
    justifyContent: 'center',
  },
});

export default App;