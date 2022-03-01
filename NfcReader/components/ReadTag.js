import React, { useState } from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import NfcManager, { NfcTech, Ndef } from 'react-native-nfc-manager';
import init from 'react_native_mqtt';
import { AsyncStorage } from '@react-native-community/async-storage';



init({
	size: 10000,
	storageBackend: AsyncStorage,
	defaultExpires: 1000 * 3600 * 24,
	enableCache: true,
	reconnect: true,
	sync: {
	}
});

export const Read = () => {

	const [text, setText] = useState('');

	const onConnect = () => {
		console.warn("connected to mqtt broker"); 
	}

	const onFailure = () => {
		console.warn("can't connect to mqtt broker");
	}

	const onConnectionLost = (responseObject) => {
		if (responseObject.errorCode !== 0) {
			console.log("onConnectionLost:" + responseObject.errorMessage);
		}
	}

	const onMessageArrived = (message) => {
		console.log("onMessageArrived:" + message.payloadString);
	}

	const client = new Paho.MQTT.Client('broker.mqttdashboard.com', 8000, 'leoGuillaumet');

	client.onConnectionLost = onConnectionLost;
	client.onMessageArrived = onMessageArrived;
	client.connect({ onSuccess: onConnect, onFailure: onFailure, useSSL: false });

	async function readNdef() {
		try {
			// register for the NFC tag with NDEF in it
			console.warn('Wait tag');
			await NfcManager.requestTechnology(NfcTech.Ndef);
			// the resolved tag object will contain `ndefMessage` property
			const tag = await NfcManager.getTag();
			console.warn('tag', tag.ndefMessage[0].payload);

			setText(Ndef.text.decodePayload(tag.ndefMessage[0].payload));
			client.send('leoGuillaumet', text);
		} catch (ex) {
			console.warn('Oops!', ex);
		} finally {
			// stop the nfc scanning
			NfcManager.cancelTechnologyRequest();
		}
	}


	return (
		<View style={styles.container}>
			<TouchableOpacity onPress={readNdef}>
				<Text>Scan a Tag</Text>
				{!!text && (<Text>{`identifiant: ${text}`}</Text>)}
			</TouchableOpacity>
		</View>
	);
}

const styles = StyleSheet.create({
	container: {
		flex: 1,
		backgroundColor: '#fff',
		alignItems: 'center',
		justifyContent: 'center',
	},
});

