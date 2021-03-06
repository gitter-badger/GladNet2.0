﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using GladNet.Common;
using GladNet.Serializer;

namespace GladNet.Common
{	
	/// <summary>
	/// Finite valid states a <see cref="NetSendable"/> can be in.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")] //We suppress this because this is going over the wire. 1 byte is far better.
	[GladNetSerializationContract]
	public enum NetSendableState : byte
	{
		Default,
		Serialized,
		Encrypted,
	}

	/// <summary>
	/// Represents a wire-ready version of the TData that provides functionality to serialize, encrypt and decrypt the TData
	/// </summary>
	/// <typeparam name="TData">The Type of encryptable and serializable data becoming wire-ready.</typeparam>
	[GladNetSerializationContract]
	public class NetSendable<TData> : IEncryptable, IDecryptable, ISerializable, IDeserializable, IShallowCloneable<NetSendable<TData>>
			where TData : class
	{
		/// <summary>
		/// Indicates the state the object is currently in.
		/// </summary>
		[GladNetMember(1, IsRequired = true)]
		public NetSendableState DataState { get; private set; }

		//This should never be serialized over the network.
		/// <summary>
		/// The TData to be manipulated or the resulting deserialized TData. Can be null depending on state.
		/// </summary>
		public TData Data { get; private set; }

		//This should be serialized over the network.
		/// <summary>
		/// The wire-ready byte[] that represents the TData in the corressponding <see cref="State"/>
		/// </summary>
		[GladNetMember(2, IsRequired = true)]
		private byte[] byteData = null;

		/// <summary>
		/// Constructor for typical <see cref="NetSendable"/> that requires an instance of TData.
		/// </summary>
		/// <param name="data">Instance of TData to be wire-ready prepared.</param>
		public NetSendable(TData data)
		{
			if (data == null)
				throw new ArgumentNullException("data", "TData data cannot be null in construction.");

			Data = data;
			DataState = NetSendableState.Default;
		}


		//Protobuf-net constructors should be marked with ncrunch no coverage to suppress it from coverage metrics
		//ncrunch: no coverage start
		/// <summary>
		/// Empty protobuf-net constuctor
		/// </summary>
		protected NetSendable()
		{

		}
		//ncrunch: no coverage end

		/// <summary>
		/// Encrypts the TData contained within this <see cref="NetSendable"/>.
		/// </summary>
		/// <param name="encryptor">Object responsible for the encryption.</param>
		/// <exception cref="InvalidOperationException">Throws when the <see cref="NetSendable"/> is not in a Serialized <see cref="NetSendableState"/></exception>
		/// <returns>Indicates if encryption was successful</returns>
		public bool Encrypt(IEncryptorStrategy encryptor)
		{
			if (encryptor == null)
				throw new ArgumentNullException("encryptor", "The encryptor cannot be null.");

			ThrowIfInvalidState(NetSendableState.Serialized);
			
			try
			{
				byteData = encryptor.Encrypt(byteData);
			}
			catch (CryptographicException)
			{
				return false;
			}

			//Check the state of the bytes
			if (byteData == null)
				return false;

			//If sucessful the data should be in an encrypted state.
			DataState = NetSendableState.Encrypted;
			return true;
		}

		/// <summary>
		/// Decrypts the TData contained within this <see cref="NetSendable"/>
		/// </summary>
		/// <param name="decryptor"></param>
		/// <exception cref="InvalidOperationException">Throws when the <see cref="NetSendable"/> is not in a Encrypted <see cref="NetSendableState"/>
		/// or if the internal byte representation is null..</exception>
		/// <returns>Indicates if decryption was successful.</returns>
		public bool Decrypt(IDecryptorStrategy decryptor)
		{

			if (decryptor == null)
				throw new ArgumentNullException("decryptor", "The decryptor cannot be null.");

			//We don't want to throw. Remote peers could send invalid packets that DOS us with exception generation.
			ThrowIfInvalidState(NetSendableState.Encrypted);

			try
			{
				byteData = decryptor.Decrypt(byteData);
			}
			catch(CryptographicException)
			{
				return false;
			}

			//Check the state of the bytes
			if (byteData == null)
				return false;

			//If successful the data should be in a serialized state.
			DataState = NetSendableState.Serialized;
			return true;
		}

		/// <summary>
		/// Serializes the TData into a byte[] for the wire.
		/// </summary>
		/// <param name="serializer">Serializer object for the serialization process.</param>
		/// <exception cref="InvalidOperationException">Throws if the data is not in Default <see cref="State"/></exception>
		/// <returns>Inidicates if serialization was successful</returns>
		public bool Serialize(ISerializerStrategy serializer)
		{
			if (serializer == null)
				throw new ArgumentNullException("serializer", "The serializer cannot be null.");

			ThrowIfInvalidState(NetSendableState.Default);

			byteData = serializer.Serialize(Data);

			//Check the state of the bytes
			if (byteData == null)
				return false;

			//If successful the data should be in a serialized state
			DataState = NetSendableState.Serialized;

			//We don't need Data anymore and it can be recreated from the current state now.
			Data = null;
			return true;
		}

		/// <summary>
		/// Deserializes the byteData into TData.
		/// </summary>
		/// <param name="deserializer">Deserializer object for the deserialization process.</param>
		/// <exception cref="InvalidOperationException">Throws if the <see cref="State"/> isn't Serialized.</exception>
		/// <returns>Indicates if deserialization was successful</returns>
		public bool Deserialize(IDeserializerStrategy deserializer)
		{
			if (deserializer == null)
				throw new ArgumentNullException("deserializer", "The derserializer cannot be null.");

			ThrowIfInvalidState(NetSendableState.Serialized);

			Data = deserializer.Deserialize<TData>(byteData);

			if (Data == null)
				return false;

			DataState = NetSendableState.Default;
			return true;
		}

		/// <summary>
		/// Throws if the state differs from the state passes as a parameter.
		/// Optionally checks, and will throw, if the internal byte[] is invalid.
		/// </summary>
		/// <param name="expectedState">The <see cref="NetSendableState"/> we expect this instance to be in.</param>
		/// <param name="checkData">Inidicates if we should validate the byte[]</param>
		/// <exception cref="InvalidOperationException">Throws if expectedState differs from current state. Optionally throws if checkData is true and the byte[] is null.</exception>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "byteData")]
		private void ThrowIfInvalidState(NetSendableState expectedState)
		{
			if(DataState != expectedState)
				throw new InvalidOperationException(GetType() + " was not in required state " + expectedState + " was in " + DataState);
		}

		/// <summary>
		/// Produces a shallow copy of this <see cref="NetSendable"/> instance.
		/// </summary>
		/// <returns>A shallow copied instance of this NetSendable. Preserves reference to byte[] for efficient multiplexing.</returns>
		public NetSendable<TData> ShallowClone()
		{
			return ((IShallowCloneable)this).ShallowClone() as NetSendable<TData>;
		}

		/// <summary>
		/// Produces a shallow copy of this <see cref="NetSendable"/> instance.
		/// </summary>
		/// <returns>A shallow copied instance of this NetSendable. Preserves reference to byte[] for efficient multiplexing.</returns>
		object IShallowCloneable.ShallowClone()
		{
			//As of Oct. 8th 2015 it is valid to MemberwiseClone for valid ShallowCopy.
			return MemberwiseClone(); //it never shouldn't be of this type
		}
	}
}
