//-----------------------------------------------------------------------------
#version 330 core

// El tipo de malla que hemos definido: eVer_Pos 
layout (location = 0) in vec3 pos;			// Atributo 0: 3 de posiciones


void main()
{
	gl_Position = vec4(pos.x, pos.y, pos.z, 1.0f);
}
//-----------------------------------------------------------------------------
