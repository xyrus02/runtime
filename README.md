# Runtime
---

This repository contains libraries and packages, which are used to build advanced applications like those using WPF and hardware accelerated graphics applications.

## License
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.*

## Included packages

### Runtime.Primitives
This package offers very basic data types like vectors and matrices, which are used by the other packages in this solution.

### Runtime.Data
This package contains tools and methods working with raw memory and fast, bytewise operations.

### Runtime.Expressions
Using this package, it is possible to build dynamic types and logic trees using the LINQ expression infrastructure as a basis.

### Runtime.Graphics
This package offers base classes for creating fast, self-drawn graphics applications using WPF.

### Runtime.Graphics.Acceleration
This package extends Runtime.Graphics by adding another layer of DirectX based hardware acceleration. It utilizes SlimDX to do so.

## Repository structure

The structure of this repository is standardized. Items shouldn't be placed on the root foder. Instead, they should be contained in one of the sub-structures below.

**src**:
The actual source code of the repository as a Microsoft Visual Studio solution

**tools**:
Script includes for PACMAN and other management shells as well as binaries for contained build tools.

## How to...

### Build
To build the solution after checkout, the following commands need to be executed in the Visual Studio Developer Shell:

	msbuild /t:restore
	msbuild /t:build

The first command restores the NuGet-packages included in the solution, the second command builds the solution.

### Create NuGet-packages
To create the NuGet-packages provided by this project, the following command needs to be executed in the PACMAN shell accessible with `shell.cmd`:

	 Publish-Packages -BuildOnly

### Create and publish NuGet-packages
To also publish the packages, omit the `-BuildOnly` switch:

	 Publish-Packages

### Change package version
The version manager is also accessible using the PACMAN shell and is used like below:

	# Raises the major version
		> Update-Version -Release
		  1.0.0 -> 2.0.0

	# Raises the minor version
		> Update-Version -Update
		  1.0.0 -> 1.1.0

	# Raises the patch version
		> Update-Version -Patch
		  1.0.0 -> 1.0.1

	# Sets a specific version
		> Update-Version -Version 2.1.0
		  1.0.0 -> 2.1.0

	# Raises the major version and adds a pre-release label 
		> Update-Version -Release -PreRelease alpha2
		  1.0.0 -> 2.0.0-alpha2

	# Raises the patch version by two steps
		> Update-Version -Patch -Increment 2
		  1.0.0 -> 1.0.2
