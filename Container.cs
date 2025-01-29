﻿using System;
using log4net;
using parry_mechanic.Content;
using parry_mechanic.Content.Parry;
using parry_mechanic.TinyIoc;
using Terraria.ModLoader;

public static class Container
{
    private static TinyIoCContainer services;

    public static void Initialize(Mod mod)
    {
        services = new TinyIoCContainer();

        RegisterServices(mod);
    }
    private static void RegisterServices(Mod mod)
    {
        services.Register(new ParryModKeybindService(mod));
        services.Register(ModContent.GetInstance<VisualModConfigService>());
        services.Register(ModContent.GetInstance<GameplayModConfigService>());
        var log = LogManager.GetLogger(mod.Name);
        services.Register(log);
    }

    public static T Resolve<T>() where T : class
    {
        if (services == null)
        {
            throw new InvalidOperationException("Service provider is not initialized. Call RegisterServices first.");
        }
        return services.Resolve<T>();
    }

    public static void Clear()
    {
        services.Dispose();
    }
}