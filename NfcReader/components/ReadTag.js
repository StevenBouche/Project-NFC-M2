import React, { useState } from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import NfcManager, { NfcTech, Ndef } from 'react-native-nfc-manager';
import * as Mqtt from 'react-native-native-mqtt';
import { Buffer } from "buffer"

const topic = 'userevent';

export const Read = () => {

	const [text, setText] = useState('');
	const client = new Mqtt.Client('tcp://172.20.10.2:9000');

	client.on(Mqtt.Event.Message, (topic, message) => {
		console.log('Mqtt Message:', topic, message.toString());
	});

	client.on(Mqtt.Event.Connect, () => {
		console.warn('MQTT Connect');
		client.subscribe([topic], [0]);
	});

	client.on(Mqtt.Event.Error, (error) => {
		console.warn('MQTT Error:', error);
	});

	client.on(Mqtt.Event.Disconnect, (cause) => {
		console.log('MQTT Disconnect:', cause);
	});

	client.connect({
		clientId: 'user',
		cleanSession: true,
	}, err => { if (err) console.warn(err); });

	async function readNdef() {
		try {
			// register for the NFC tag with NDEF in it
			console.warn('Wait tag');
			await NfcManager.requestTechnology(NfcTech.Ndef);
			// the resolved tag object will contain `ndefMessage` property
			const tag = await NfcManager.getTag();
			console.warn('tag', Ndef.text.decodePayload(tag.ndefMessage[0].payload));

			setText(Ndef.text.decodePayload(tag.ndefMessage[0].payload));
			const msg = Buffer.from(Ndef.text.decodePayload(tag.ndefMessage[0].payload), "utf-8");
			// convert buffer to string
			const resultStr = msg.toString();

			console.warn("BufferString:", resultStr); //Hey. this is a string!

			client.publish(topic, msg, 0);
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

