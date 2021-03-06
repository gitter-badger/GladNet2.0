﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Implementer is an object that can be encrypted.
	/// </summary>
	public interface IEncryptable
	{
		/// <summary>
		/// Attempts to encrypt the raw data within the implementing type.
		/// </summary>
		/// <param name="encryptor">The encryptor object that handles the specifics of encryption.</param>
		/// <exception cref="CryptographicException">Throws if encryption fails.</exception>
		/// <returns>Indicates if encryption was successful.</returns>
		bool Encrypt(IEncryptorStrategy encryptor);
	}
}
