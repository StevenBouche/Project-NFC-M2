import React, { useState } from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import NfcManager, { NfcTech, Ndef } from 'react-native-nfc-manager';
import init from 'react_native_mqtt';
import { AsyncStorage } from '@react-native-community/async-storage';

export const Read = () => {

	const [text, setText] = useState('');
	async function readNdef() {
		try {
			// register for the NFC tag with NDEF in it
			console.warn('Wait tag');
			await NfcManager.requestTechnology(NfcTech.Ndef);
			// the resolved tag object will contain `ndefMessage` property
			const tag = await NfcManager.getTag();
			console.warn('tag', tag.ndefMessage[0].payload);

			setText(Ndef.text.decodePayload(tag.ndefMessage[0].payload));
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

