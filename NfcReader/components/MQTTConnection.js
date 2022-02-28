import React, { useState } from 'react';
import init from 'react_native_mqtt';
import { StyleSheet, Text, View } from 'react-native';
import AsyncStorage from '@react-native-community/async-storage'

init({
	size: 10000,
	storageBackend: AsyncStorage,
	defaultExpires: 1000 * 3600 * 24,
	enableCache: true,
	sync: {},
});

const styles = StyleSheet.create({
	container: {
		width: '100%',
		height: '100%',
	},
});

export const MqttLog = ({ }) => {

	client.onConnectionLost = this.onConnectionLost;
	client.onMessageArrived = this.onMessageArrived;
	client.connect({ onSuccess: this.onConnect, useSSL: true });

	const [client, setClient] = useState(new Paho.MQTT.Client('iot.eclipse.org', 443, 'uname'));
	const [text, setText] = useState(['...'])



	const pushText = (entry) => {
		setText(prev => [...prev, entry]);
	};

	const onConnect = () => {
		client.subscribe('WORLD');
		pushText('connected');
	};

	const onConnectionLost = (responseObject) => {
		if (responseObject.errorCode !== 0) {
			pushText(`connection lost: ${responseObject.errorMessage}`);
		}
	};

	const onMessageArrived = (message) => {
		pushText(`new message: ${message.payloadString}`);
	};

	return (
		<View style={styles.container}>
			{text.map(entry => <Text>{entry}</Text>)}
		</View>
	);
}